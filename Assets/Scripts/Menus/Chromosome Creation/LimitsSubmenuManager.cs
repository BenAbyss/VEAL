using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LimitsSubmenuManager : MenuManager
{
    public static event Action<ChromosomeLimits, int> VariableUpdated;
    
    [SerializeField] private GameObject numbersSubmenu;
    [SerializeField] private GameObject textSubmenu;
    [SerializeField] private GameObject enumSubmenu;
    [SerializeField] private GameObject noLimitsText;
    [SerializeField] private GameObject invalidValuesScroller;
    [SerializeField] private GameObject enumeratorScroller;
    [SerializeField] private GameObject invalidPrefab;
    [SerializeField] private GameObject enumPrefab;
    [SerializeField] private GameObject[] optionalItems;
    [SerializeField] private SerializableStringGameObjDict textFields;

    public Dictionary<int, List<string>> Invalids = new Dictionary<int, List<string>>();
    public Dictionary<int, List<(string, float)>> Enumerators = new Dictionary<int, List<(string, float)>>();
    private int _varId;
    private VarType _currentType;

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        InvalidValueDataPiece.DeletedInvalidValue += DataPieceRemoved;
        EnumeratorDataPiece.DeletedEnumeratorValue += DataPieceRemoved;
        InputManager.CancelAction += CloseMenu;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        InvalidValueDataPiece.DeletedInvalidValue -= DataPieceRemoved;
        EnumeratorDataPiece.DeletedEnumeratorValue -= DataPieceRemoved;
        InputManager.CancelAction -= CloseMenu;
    }
    
    
    
    /// <summary>
    /// Method <c>CloseMenu</c> saves the details and closes the menu.
    /// </summary>
    public override void CloseMenu()
    {
        SaveLimits();
        base.CloseMenu();
    }
    
    /// <summary>
    /// Method <c>CloseMenuUnsaved</c> closes the menu.
    /// </summary>
    public void CloseMenuUnsaved()
    {
        base.CloseMenu();
    }
    
    
    
    /// <summary>
    /// Method <c>SetupScene</c> sets up the scene appropriately based on it's type.
    /// <param name="variable">The variable's limits object to extract current data from.</param>
    /// <param name="var_id">The variables' ID.</param>
    /// <param name="type">The type of the variable.</param>
    /// </summary>
     public void SetupScene(ChromosomeLimits curr_limits, int var_id, VarType type)
    {
        // all menus are initially disabled to avoid an over-cluttered switch statement
        DisableAll();
        _currentType = type;
        _varId = var_id;
        
        switch (type)
        {
            case VarType.Integer:
                numbersSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                UpdateSceneData(curr_limits.NumVal, eq: curr_limits.Equation);
                break;
            case VarType.Float:
                numbersSubmenu.SetActive(true);
                UpdateSceneData(curr_limits.NumVal, eq: curr_limits.Equation, dec_places: curr_limits.DecPlaces);
                break;
            case VarType.String:
                textSubmenu.SetActive(true);
                UpdateSceneData(str_length: curr_limits.StrLength, invalid_strings: curr_limits.InvalidStrings);
                break;
            case VarType.Character:
                textSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                UpdateSceneData(invalid_strings: curr_limits.InvalidStrings);
                break;
            case VarType.Enumerator:
                enumSubmenu.SetActive(true);
                UpdateSceneData(enum_options: Enumerators[_varId]);
                break;
            default:
                noLimitsText.SetActive(true);
                break;
        }
    }

    private void UpdateSceneData(Dictionary<string, int> num_val = null, Dictionary<string, int> str_length = null, 
        string eq = null, int dec_places = -1, List<string> invalid_strings = null, List<(string, float)> enum_options = null)
    {
        if (num_val != null)
        {
            SetFieldData("MinInput", num_val.ContainsKey("Min") ? num_val["Min"].ToString() : "");
            SetFieldData("MaxInput", num_val.ContainsKey("Max") ? num_val["Max"].ToString() : "");
        }

        if (str_length != null)
        {
            SetFieldData("MinLengthInput", str_length.ContainsKey("Min") ? str_length["Min"].ToString() : "");
            SetFieldData("MaxLengthInput", str_length.ContainsKey("Min") ? str_length["Max"].ToString() : "");
        }
        
        SetFieldData("DecInput", dec_places != -1 ? dec_places.ToString() : "");
        SetFieldData("EqInput", eq ?? "");

        ClearScroller(invalidValuesScroller);
        if (invalid_strings != null)
        {
            foreach (var invalid_string in invalid_strings)
            {
                AddNewInvalid(invalid_string);
            }
        }
        
        ClearScroller(enumeratorScroller);
        if (enum_options != null)
        {
            foreach (var enum_option in enum_options)
            {
                AddNewEnum(enum_option);
            }
        }
    }

    /// <summary>
    /// Method <c>SaveLimits</c> saves the currently entered limits to the current variable.
    /// </summary>
    private void SaveLimits()
    {
        if (!Enum.IsDefined(typeof(VarType), _currentType)) return;
        ChromosomeLimits limits;
        
        switch (_currentType)
        {
            case VarType.Integer:
                numbersSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                limits = new ChromosomeLimits(_currentType, CreateMinMaxDict("MinInput", "MaxInput"), 
                    eq: GetFieldData("EqInput"));
                break;
            case VarType.Float:
                numbersSubmenu.SetActive(true);
                limits = new ChromosomeLimits(_currentType, CreateMinMaxDict("MinInput", "MaxInput"), 
                    eq: GetFieldData("EqInput"), dec_places: GetIntFieldData("DecInput"));
                break;
            case VarType.String:
                textSubmenu.SetActive(true);
                limits = new ChromosomeLimits(_currentType, invalid_strings: Invalids[_varId],
                    str_length: CreateMinMaxDict("MinLengthInput", "MaxLengthInput"));
                break;
            case VarType.Character:
                textSubmenu.SetActive(true);
                ToggleOptionalItems(false);
                limits = new ChromosomeLimits(_currentType, invalid_strings: Invalids[_varId]);
                break;
            case VarType.Enumerator:
                enumSubmenu.SetActive(true);
                // for each enumerator, update it to the new values
                UpdateEnums();
                limits = new ChromosomeLimits(_currentType, enum_options: Enumerators[_varId]);
                break;
            default:
                noLimitsText.SetActive(true);
                limits = new ChromosomeLimits(_currentType);
                break;
        }

        VariableUpdated?.Invoke(limits, _varId);
    }



    /// <summary>
    /// Method <c>EnumAdded</c> adds the new enumeration value from the input box to the list,
    /// and clears the input box.
    /// </summary>
    public void EnumAdded()
    {
        var field = textFields["EnumValuesInput"].GetComponent<TMP_InputField>();
        var new_enum = (field.text, 0);
        Enumerators[_varId].Add(new_enum);
        AddNewEnum(new_enum);
        field.text = "";
    }
    
    /// <summary>
    /// Method <c>InvalidAdded</c> adds the new invalid character/string from the input box to the list,
    /// and clears the input box.
    /// </summary>
    public void InvalidAdded()
    {
        var field = textFields["InvalidValuesInput"].GetComponent<TMP_InputField>();
        Invalids[_varId].Add(field.text);
        AddNewInvalid(field.text);
        field.text = "";
    }

    /// <summary>
    /// Method <c>AddNewInvalid</c> adds the new invalid character/string to the list.
    /// <param name="text">The invalid text.</param>
    /// </summary>
    private void AddNewInvalid(string text)
    {
        var built_prefab = Instantiate(invalidPrefab, 
            invalidValuesScroller.GetComponent<VerticalLayoutGroup>().transform, false);
        built_prefab.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = text;
        AdjustScroller();
    }
    
    /// <summary>
    /// Method <c>AddNewEnum</c> adds the new enumerator to the list.
    /// <param name="enumerator">The new enumerator.</param>
    /// </summary>
    private void AddNewEnum((string, float) enumerator)
    {
        var built_prefab = Instantiate(enumPrefab, 
            enumeratorScroller.GetComponent<VerticalLayoutGroup>().transform, false);
        built_prefab.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = enumerator.Item1;
        built_prefab.transform.Find("WeightInput").GetComponent<TMP_InputField>().text = enumerator.Item2.ToString();
        AdjustScroller();
    }

    /// <summary>
    /// Method <c>DataPieceRemoved</c> deletes the given data piece.
    /// <param name="type">The type of data to delete.</param>
    /// <param name="data_piece">The data piece to delete.</param>
    /// </summary>
    private void DataPieceRemoved(string type, string data_piece)
    {
        switch (type)
        {
            case "enumerator":
                Enumerators[_varId].Remove(Enumerators[_varId].First(item => item.Item1 == data_piece));
                break;
            case "invalid":
                Invalids[_varId].Remove(data_piece);
                break;
        }

        AdjustScroller();
    }
    
    /// <summary>
    /// Method <c>ClearScroller</c> clears the given strings scroller.
    /// <param name="scroller">The scroller to clear.</param>
    /// </summary>
    private void ClearScroller(GameObject scroller)
    {
        foreach (Transform child in scroller.transform)
        {
            Destroy(child.gameObject);
        }
    }

    
    
    /// <summary>
    /// Method <c>MinInput</c> appropriately adjusts the input value for MinInput, used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void MinInput(string input)
     {
         InputLimit(input, "MinInput", @"[^.0-9]");
     }
     
    /// <summary>
    /// Method <c>MaxInput</c> appropriately adjusts the input value for MaxInput, used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void MaxInput(string input)
     {
         InputLimit(input, "MaxInput", @"[^.0-9]");
     }
     
    /// <summary>
    /// Method <c>MinLengthInput</c> appropriately adjusts the input value for MinLengthInput,
    /// used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void MinLengthInput(string input)
     {
         InputLimit(input, "MinLengthInput", @"[^0-9]");
     }
     
    /// <summary>
    /// Method <c>MaxLengthInput</c> appropriately adjusts the input value for MaxLengthInput,
    /// used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void MaxLengthInput(string input)
     {
         InputLimit(input, "MaxLengthInput", @"[^0-9]");
     }
     
    /// <summary>
    /// Method <c>DecInput</c> appropriately adjusts the input value for DecInput, used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void DecInput(string input)
     {
         InputLimit(input, "DecInput", @"[^0-9]");
     }

    /// <summary>
    /// Method <c>EqInput</c> appropriately adjusts the input value for EqInput, used for OnValueChanged calls.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void EqInput(string input)
     {
         InputLimit(input, "EqInput", @"[^.*()/+-=<>%x0-9]");
     }
     
    /// <summary>
    /// Method <c>InvalidValuesInput</c> limits the length of an input to one character, if appropriate.
    /// <param name="input">The unedited input.</param>
    /// </summary>
     public void InvalidValuesInput(string input)
     {
         if (_currentType == VarType.Character && input.Length > 0)
         {
             textFields["InvalidValuesInput"].GetComponent<TMP_InputField>().text = input[input.Length-1].ToString();
         }
     }

    
    
    /// <summary>
    /// Method <c>GetIntFieldData</c> gets the integer value stored in an input field.
    /// <param name="field">The input field to access.</param>
    /// <returns>The integer value stored in the input field</returns>
    /// </summary>
    private int GetIntFieldData(string field)
    {
        var data = GetFieldData(field);
        if (data != "")
        {
            return Convert.ToInt32(data);
        }
        return -1;
    }
    
    /// <summary>
    /// Method <c>GetFieldData</c> gets the text string stored in an input field.
    /// <param name="field">The input field to access.</param>
    /// <returns>The text string stored in the input field</returns>
    /// </summary>
    private string GetFieldData(string field)
    {
        return textFields[field].GetComponent<TMP_InputField>().text;
    }
    
    /// <summary>
    /// Method <c>SetFieldData</c> sets the text string stored in an input field.
    /// <param name="field">The input field to access.</param>
    /// <param name="new_data">The string to set it to.</param>
    /// </summary>
    private void SetFieldData(string field, string new_data)
    {
        textFields[field].GetComponent<TMP_InputField>().text = new_data;
    }
    
    /// <summary>
    /// Method <c>InputLimit</c> adjusts an input to accord with a given limit.
    /// <param name="input">The unedited input.</param>
    /// <param name="text_field">The name of the text field it belongs to.</param>
    /// <param name="disallowed_vals">Characters disallowed from the input.</param>
    /// </summary>
    private void InputLimit(string input, string text_field, string disallowed_vals)
    {
        textFields[text_field].GetComponent<TMP_InputField>().text = 
            Regex.Replace(input, disallowed_vals, "");
    }

    /// <summary>
    /// Method <c>DisableAll</c> disables all the possible submenus.
    /// </summary>
    private void DisableAll()
    {
        // optionals are toggled to true so that they'll be already enabled when their menu is enabled
        ToggleOptionalItems(true);
        numbersSubmenu.SetActive(false);
        textSubmenu.SetActive(false);
        noLimitsText.SetActive(false);
    }

    /// <summary>
    /// Method <c>ToggleOptionalItems</c> toggles the availability of the optional items.
    /// <param name="toggle">How to toggle it.</param>
    /// </summary>
    private void ToggleOptionalItems(bool toggle)
    {
        foreach (var item in optionalItems)
        {
            item.SetActive(toggle);
        }
    }
    
    /// <summary>
    /// Method <c>AdjustScroller</c> adjusts the scroller to the appropriate size.
    /// </summary>
    private void AdjustScroller()
    {
        var trans = invalidValuesScroller.GetComponent<RectTransform>();
        var grid = invalidValuesScroller.GetComponent<VerticalLayoutGroup>();
        trans.sizeDelta = new Vector2(trans.sizeDelta[0],
            ((RectTransform) invalidPrefab.transform).rect.height * Invalids[_varId].Count
            + grid.spacing * Invalids[_varId].Count + grid.padding.top * 2);
    }

    /// <summary>
    /// Method <c>CreateMinMaxDict</c> creates a dictionary for minimum and maximum inputted values,
    /// providing they exist.
    /// <param name="min">The name of the minimum input text field.</param>
    /// <param name="max">The name of the maximum input text field.</param>
    /// <returns>The created dictionary</returns>
    /// </summary>
    private Dictionary<string, int> CreateMinMaxDict(string min, string max)
    {
        var dict = new Dictionary<string, int>();
        if (GetFieldData(min) != "")
        {
            dict["Min"] = GetIntFieldData(min);
        }
        if (GetFieldData(max) != "")
        {
            dict["Max"] = GetIntFieldData(max);
        }

        return dict;
    }

    /// <summary>
    /// Method <c>UpdateEnums</c> updates the enumerators to be the appropriate current values.
    /// </summary>
    private void UpdateEnums()
    {
        var enums = new List<(string, float)>();
        enums.AddRange(from Transform child in enumeratorScroller.transform 
            select child.gameObject.GetComponent<EnumeratorDataPiece>().GetUpdatedEnum());
        Enumerators[_varId] = enums;
    }
}