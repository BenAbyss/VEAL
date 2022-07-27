using System;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class FitnessDataPiece : MonoBehaviour
{
    public static int VariablesCount;
    public int variableId;
    [SerializeField] private TextMeshProUGUI varName;
    [SerializeField] private TMP_Dropdown calcDropdown;
    [SerializeField] private TMP_InputField valInput;
    [SerializeField] private TMP_InputField weightInput;

    /// <summary>
    /// Method <c>Start</c> sets up the variables.
    /// </summary>
    public void Start()
    {
        if (variableId != 0) return;
        VariablesCount++;
        variableId = VariablesCount;
        DropdownChanged(0);
    }
    
    /// <summary>
    /// Method <c>UpdateFitness</c> updates the inputted values to match that of the provided fitness.
    /// <param name="var_name">The variable to match the fitness of.</param>
    /// <param name="type">The new type of the fitness.</param>
    /// <param name="weighting">The new weighting of the fitness.</param>
    /// <param name="target">The new target of the fitness.</param>
    /// </summary>
    public void UpdateFitness(string var_name, FitnessType type, float weighting, float target=0)
    {
        varName.text = var_name;
        calcDropdown.value = calcDropdown.options.FindIndex(option => 
            option.text.Replace(" ", "") == type.ToString());
        valInput.text = target.ToString();
        weightInput.text = weighting.ToString();
    }

    
    
    /// <summary>
    /// Method <c>SetVarName</c> sets the name of the variable to represent.
    /// <param name="new_name">The new variable name.</param>
    /// </summary>
    public void SetVarName(string new_name)
    {
        varName.text = new_name;
    }
    
    /// <summary>
    /// Method <c>GetId</c> gets the variable's ID.
    /// <returns>The variables' ID.</returns>
    /// </summary>
    public int GetId()
    {
        return Math.Max(variableId, 1);
    }
    
    /// <summary>
    /// Method <c>GetCalcType</c> gets the variable's calculation type.
    /// <returns>The variables' calculation type.</returns>
    /// </summary>
    private FitnessType GetCalcType()
    {
        return (FitnessType) Enum.Parse(typeof(FitnessType),
            calcDropdown.options[calcDropdown.value].text.Replace(" ", ""));
    }
    
    /// <summary>
    /// Method <c>GetCalcVal</c> gets the variable's calculation value.
    /// <returns>The variables' calculation value.</returns>
    /// </summary>
    private float GetCalcVal()
    {
        var txt = valInput.text;
        return (float) (txt != "" ? Convert.ToDouble(txt) : 0);
    }

    /// <summary>
    /// Method <c>GetWeighting</c> gets the variable's weighting.
    /// <returns>The variables' weighting.</returns>
    /// </summary>
    private float GetWeighting()
    {
        var txt = weightInput.text;
        return (float) (txt != "" ? Convert.ToDouble(txt) : 0);
    }
    
    
    
    /// <summary>
    /// Method <c>DropdownChanged</c> enables the customise button if the struct datatype is chosen.
    /// <param name="pos">The dropdown position selected.</param>
    /// </summary>
    public void DropdownChanged(int pos)
    {
        valInput.gameObject.SetActive(
            new[] {"Closer To", "Further From"}.Contains(calcDropdown.options[calcDropdown.value].text));
    }

    /// <summary>
    /// Method <c>ValueInput</c> adjusts the input to one containing only numbers and '.'.
    /// <param name="input">The unedited input.</param>
    /// </summary>
    public void ValueInput(string input)
    {
        valInput.text = Regex.Replace(input, @"[^.0-9]", "");
    }
    
    /// <summary>
    /// Method <c>WeightInput</c> adjusts the input to one containing only numbers and '.'.
    /// <param name="input">The unedited input.</param>
    /// </summary>
    public void WeightInput(string input)
    {
        weightInput.text = Regex.Replace(input, @"[^.0-9]", "");
    }

    public ChromosomeVariable ApplyFitness(ChromosomeVariable variable)
    {
        variable.SetupFitness(GetCalcType(), GetCalcVal(), GetWeighting());
        return variable;
    }
}