using TMPro;
using UnityEngine;

public class VariableDataPiece : MonoBehaviour
{
    private TMP_InputField _nameInput;
    private TMP_Dropdown _typeInput;
    private TextMeshProUGUI _limitsText;
    private GameObject _typeBtn;

    /// <summary>
    /// Method <c>Start</c> sets up the variables.
    /// </summary>
    public void Start()
    {
        _nameInput = GetComponentInChildren<TMP_InputField>();
        _typeInput = GetComponentInChildren<TMP_Dropdown>();
        _limitsText = GameObject.Find("LimitsText").GetComponent<TextMeshProUGUI>();
        _typeBtn = GameObject.Find("TypeBtn");
        _typeBtn.SetActive(false);
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
    public string GetVarType()
    {
        return _typeInput.options[_typeInput.value].text;
    }

    /// <summary>
    /// Method <c>GetLimits</c> gets the variable's limits.
    /// <returns>The variables' limits.</returns>
    /// </summary>
    public string GetLimits()
    {
        return _limitsText.text;
    }


    /// <summary>
    /// Method <c>DropdownChanged</c> enables the customise button if the struct datatype is chosen.
    /// <param name="pos">The dropdown position selected.</param>
    /// </summary>
    public void DropdownChanged(int pos)
    {
        Debug.Log(_typeInput.options[_typeInput.value].text);
        if (_typeInput.options[_typeInput.value].text == "Struct")
        {
            _typeBtn.SetActive(true);
            _limitsText.text = "Set limits internally";
        }
        else
        {
            _typeBtn.SetActive(false);
        }
    }
}