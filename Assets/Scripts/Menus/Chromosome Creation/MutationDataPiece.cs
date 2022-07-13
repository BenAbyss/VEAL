using TMPro;
using UnityEngine;

public class MutationDataPiece : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI placeholderName;
    private string _name;
    
    /// <summary>
    /// Method <c>Start</c> sets up the object.
    /// </summary>
    public void Start()
    {
        _name = "Mutation " + ChromosomeCreationManager.MutationCount;
        placeholderName.text = _name;
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
        _name = new_name;
    }
}