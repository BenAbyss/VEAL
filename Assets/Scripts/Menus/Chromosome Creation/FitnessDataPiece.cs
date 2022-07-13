using TMPro;
using UnityEngine;

public class FitnessDataPiece : MonoBehaviour
{
    private TextMeshProUGUI _varName;
    private TMP_Dropdown _calcDropdown;
    private TMP_InputField _valInput;
    private TMP_InputField _weightInput;

    /// <summary>
    /// Method <c>Start</c> sets up the variables.
    /// </summary>
    public void Start()
    {
        var trans = gameObject.transform;
        _varName = trans.Find("VariableName").gameObject.GetComponent<TextMeshProUGUI>();
        _calcDropdown = GetComponentInChildren<TMP_Dropdown>();
        _valInput = trans.Find("CalcValInput").gameObject.GetComponent<TMP_InputField>();
        _weightInput = trans.Find("WeightInput").gameObject.GetComponent<TMP_InputField>();
    }

    /// <summary>
    /// Method <c>SetVarName</c> sets the name of the variable to represent.
    /// <param name="new_name">The new variable name.</param>
    /// </summary>
    public void SetVarName(string new_name)
    {
        _varName.text = new_name;
    }
    
    /// <summary>
    /// Method <c>GetCalcType</c> gets the variable's calculation type.
    /// <returns>The variables' calculation type.</returns>
    /// </summary>
    public string GetCalcType()
    {
        return _calcDropdown.itemText.text;
    }
    
    /// <summary>
    /// Method <c>GetCalcVal</c> gets the variable's calculation value.
    /// <returns>The variables' calculation value.</returns>
    /// </summary>
    public string GetCalcVal()
    {
        return _valInput.text;
    }

    /// <summary>
    /// Method <c>GetWeighting</c> gets the variable's weighting.
    /// <returns>The variables' weighting.</returns>
    /// </summary>
    public string GetWeighting()
    {
        return _weightInput.text;
    }
}