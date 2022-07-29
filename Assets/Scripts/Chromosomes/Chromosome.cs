using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public class Chromosome
{
     
    public List<ChromosomeVariable> variables;
    public List<string> mutationNames;
    public List<string> crossoverNames;
    public string parentChromosome;

    public Chromosome()
    {
        variables = new List<ChromosomeVariable>();
        mutationNames = new List<string>();
        crossoverNames = new List<string>();
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        CrossoverDataPiece.NameChanged += UpdateName;
        MutationDataPiece.NameChanged += UpdateName;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        CrossoverDataPiece.NameChanged -= UpdateName;
        MutationDataPiece.NameChanged -= UpdateName;
    }

    private void UpdateName(string action_type, string old_name, string new_name)
    {
        action_type = action_type.ToLower() + "Names";
        ((List<string>) GetType().GetField(action_type).GetValue(this)).Remove(old_name);
        ((List<string>) GetType().GetField(action_type).GetValue(this)).Add(new_name);
    }
}