using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class EnumeratorDataPiece : MonoBehaviour
{
    public static event Action<string, string> DeletedEnumeratorValue;
    [SerializeField] private TextMeshProUGUI value;
    [SerializeField] private TMP_InputField weightInput;
    
    /// <summary>
    /// Method <c>Delete</c> calls it's deletion before deleting the game object.
    /// </summary>
    public void Delete()
    {
        DeletedEnumeratorValue?.Invoke("enumerator", value.text);
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Method <c>GetWeighting</c> gets the value's weighting.
    /// <returns>The value's weighting.</returns>
    /// </summary>
    public float GetWeighting()
    {
        var txt = weightInput.text;
        return (float) (txt != "" ? Convert.ToDouble(txt) : 0);
    }

    /// <summary>
    /// Method <c>WeightInput</c> adjusts the input to one containing only numbers and '.'.
    /// <param name="input">The unedited input.</param>
    /// </summary>
    public void WeightInput(string input)
    {
        weightInput.text = Regex.Replace(input, @"[^.0-9]", "");
    }

    /// <summary>
    /// Method <c>GetUpdatedEnum</c> gets the value's name and weighting.
    /// <returns>The value's name and weighting.</returns>
    /// </summary>
    public (string, float) GetUpdatedEnum()
    {
        return (value.text, GetWeighting());
    }
}