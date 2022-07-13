using TMPro;
using UnityEngine;

public class CrossoverDataPiece : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeholderName;
    private string _name;
    
    /// <summary>
    /// Method <c>Start</c> sets up the object.
    /// </summary>
    public void Start()
    {
        _name = "Crossover " + ChromosomeCreationManager.CrossoverCount;
        placeholderName.text = _name;
    }

    /// <summary>
    /// Method <c>GetName</c> gets the crossover's name.
    /// <returns>The crossovers' name.</returns>
    /// </summary>
    public string GetName()
    {
        return _name;
    }



    /// <summary>
    /// Method <c>NewName</c> sets the crossovers' name to the new entered name.
    /// <param name="new_name">The new entered name.</param>
    /// </summary>
    public void NewName(string new_name)
    {
        _name = new_name;
    }
}