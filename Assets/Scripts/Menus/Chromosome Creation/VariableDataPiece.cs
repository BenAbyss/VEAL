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

    public void OpenLimits()
    {
        _manager.OpenLimitsMenu(GetId(), GetVarType());
    }
}