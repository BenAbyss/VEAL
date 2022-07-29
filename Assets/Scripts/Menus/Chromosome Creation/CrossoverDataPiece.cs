using System;
using TMPro;
using UnityEngine;

public class CrossoverDataPiece : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeholderName;
    public static event Action<string, string, string> LoadInternals;
    public static event Action<string, string, string> NameChanged;
    public string chromosomeName;
    private string _name;
    
    /// <summary>
    /// Method <c>Start</c> sets up the object.
    /// </summary>
    public void Start()
    {
        var par_name = ChromosomeCreationManager.ParentChromosome;
        chromosomeName =  string.IsNullOrEmpty(par_name) ? "MainChromosome" : par_name;
        _name = "Crossover " + ChromosomeCreationManager.CrossoverCount;
        placeholderName.text = _name;
    }
    
    /// <summary>
    /// Method <c>LoadUpInternals</c> invokes a call to load up the internals this button stores.
    /// </summary>
    public void LoadUpInternals()
    {
        LoadInternals?.Invoke(_name, chromosomeName, "Crossover");
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
        NameChanged?.Invoke("Crossover", _name, new_name);
        _name = new_name;
    }
}