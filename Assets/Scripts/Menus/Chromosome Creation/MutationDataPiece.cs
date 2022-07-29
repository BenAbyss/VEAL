using System;
using TMPro;
using UnityEngine;

public class MutationDataPiece : MonoBehaviour
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
        _name = "Mutation " + ChromosomeCreationManager.MutationCount;
        placeholderName.text = _name;
    }

    /// <summary>
    /// Method <c>LoadUpInternals</c> invokes a call to load up the internals this button stores.
    /// </summary>
    public void LoadUpInternals()
    {
        LoadInternals?.Invoke(_name, chromosomeName, "Mutation");
    }
    
    /// <summary>
    /// Method <c>GetName</c> gets the mutation's name.
    /// <returns>The mutations name.</returns>
    /// </summary>
    public string GetName()
    {
        return _name;
    }



    /// <summary>
    /// Method <c>NewName</c> sets the mutations name to the new entered name.
    /// <param name="new_name">The new entered name.</param>
    /// </summary>
    public void NewName(string new_name)
    {
        NameChanged?.Invoke("Mutation", _name, new_name);
        _name = new_name;
    }
}