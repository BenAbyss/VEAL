using System;
using TMPro;
using UnityEngine;

public class VariableDataPiece : MonoBehaviour
{
    public static event Action<int> DeletedVariable;
    [SerializeField] private TextMeshProUGUI limitsText;
    [SerializeField] private GameObject typeBtn;
    
    public static int VariablesCount;
    private int _variableId;
    private ChromosomeCreationManager _manager;
    private TMP_InputField _nameInput;
    private TMP_Dropdown _typeInput;

    /// <summary>
    /// Method <c>Start</c> sets up the variables.
    /// </summary>
    public void Start()
    {
        if (_variableId != 0) return;
        VariablesCount++;
        _variableId = VariablesCount;
        
        _nameInput = GetComponentInChildren<TMP_InputField>();
        _typeInput = GetComponentInChildren<TMP_Dropdown>();
        typeBtn.SetActive(false);
    }
    
    /// <summary>
    /// Method <c>Delete</c> calls it's deletion before deleting the game object.
    /// </summary>
    public void Delete()
    {
        DeletedVariable?.Invoke(_variableId);
        Destroy(gameObject);
    }



    /// <summary>
    /// Method <c>GetId</c> gets the variable's ID.
    /// <returns>The variables' ID.</returns>
    /// </summary>
    public int GetId()
    {
        return Math.Max(_variableId, 1);
    }
    
    /// <summary>
    /// Method <c>GetName</c> gets the variable's name.
    /// <returns>The variables' name.</returns>
    /// </summary>
    public string GetName()
    {
        return _nameInput.text;
    }

    /// <summary>
    /// Method <c>SetName</c> sets the variable's name.
    /// <param name="new_name">The new name.</param>
    /// </summary>
    public void SetName(string new_name)
    {
        _nameInput.text = new_name;
    }

    /// <summary>
    /// Method <c>GetVarType</c> gets the variable's type.
    /// <returns>The variables' type.</returns>
    /// </summary>
    public VarType GetVarType()
    {
        return (VarType)Enum.Parse(typeof(VarType), _typeInput.options[_typeInput.value].text);
    }

    /// <summary>
    /// Method <c>GetLimits</c> gets the variable's limits.
    /// <returns>The variables' limits.</returns>
    /// </summary>
    public string GetLimits()
    {
        return limitsText.text;
    }

    /// <summary>
    /// Method <c>SetManager</c> stores the manager of the game object.
    /// <param name="manager">The manager to utilise.</param>
    /// </summary>
    public void SetManager(ChromosomeCreationManager manager)
    {
        _manager = manager;
    }



    /// <summary>
    /// Method <c>NameChanged</c> calls the manager to change the name.
    /// <param name="new_name">The newly entered name.</param>
    /// </summary>
    public void NameChanged(string new_name)
    {
        _manager.NewVarName(new_name, _variableId);
    }
    
    /// <summary>
    /// Method <c>DropdownChanged</c> enables the customise button if the struct datatype is chosen.
    /// <param name="pos">The dropdown position selected.</param>
    /// </summary>
    public void DropdownChanged(int pos)
    {
        if (_typeInput.options[_typeInput.value].text == "Struct")
        {
            typeBtn.SetActive(true);
            limitsText.text = "Set limits internally";
        }
        else
        {
            typeBtn.SetActive(false);
        }
    }

    /// <summary>
    /// Method <c>OpenLimits</c> opens up the limits menu.
    /// </summary>
    public void OpenLimits()
    {
        _manager.OpenLimitsMenu(GetId(), GetVarType());
    }

    /// <summary>
    /// Method <c>UpdateText</c> updates the brief limits text appropriately.
    /// <param name="limits">The limits to base the text on.</param>
    /// </summary>
    public void UpdateText(ChromosomeLimits limits)
    {
        var new_text = "";

        if (limits.NumVal != null)
        {
            if (limits.NumVal.ContainsKey("Min") && limits.NumVal.ContainsKey("Max"))
            {
                new_text += $"range ({limits.NumVal["Min"]}, {limits.NumVal["Max"]}), ";
            } 
            else if (limits.NumVal.ContainsKey("Min"))
            {
                new_text += $"min {limits.NumVal["Min"]}, ";
            } 
            else if (limits.NumVal.ContainsKey("Max"))
            {
                new_text += $"max {limits.NumVal["Max"]}, ";
            }
        }
        
        if (limits.StrLength != null)
        {
            if (limits.StrLength.ContainsKey("Min") && limits.StrLength.ContainsKey("Max"))
            {
                new_text += $"range ({limits.StrLength["Min"]}, {limits.StrLength["Max"]}), ";
            } 
            else if (limits.StrLength.ContainsKey("Min"))
            {
                new_text += $"min {limits.StrLength["Min"]}, ";
            } 
            else if (limits.StrLength.ContainsKey("Max"))
            {
                new_text += $"min {limits.StrLength["Max"]}, ";
            }
        }
        
        new_text += (limits.Equation != "" ? "must meet eq, " : "");
        new_text += (limits.DecPlaces != -1 ? $"{limits.DecPlaces}d.p., " : "");
        new_text += (limits.InvalidStrings != null ?  "not invalid string, " : "");

        // if no limits were entered, state so
        new_text = (new_text != "" ?  new_text : "No Limits");

        limitsText.text = new_text.Substring(0, new_text.Length-2);
    }
}