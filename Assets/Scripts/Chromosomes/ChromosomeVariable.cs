using System;
using UnityEngine;

[Serializable] 
public class ChromosomeVariable
{
    public string chrName;
    public string internalsName;
    public VarType type;
    public ChromosomeLimits limits;
    public FitnessType fitnessCalc;
    public float fitnessTargetVal; // wont be string
    public float fitnessWeight;

    /// <summary>
    /// Method <c>ChromosomeVariable</c> changes the variables' type.
    /// <param name="new_type">The new type of the variable.</param>
    /// </summary>
    public ChromosomeVariable(VarType new_type)
    {
        type = new_type;
        limits = new ChromosomeLimits(new_type);
    }
    
    

    /// <summary>
    /// Method <c>SetupFitness</c> sets up the appropriate fitness variables.
    /// <param name="fitness_type">The type of fitness calculation to run.</param>
    /// <param name="target">The target value for the calculation.</param>
    /// <param name="weight">The overall weighting this variable holds.</param>
    /// </summary>
    public void SetupFitness(FitnessType fitness_type, float target, float weight)
    {
        fitnessCalc = fitness_type;
        fitnessTargetVal = target;
        fitnessWeight = weight;
    }

    /// <summary>
    /// Method <c>SetupFitness</c> updates the internals' name.
    /// </summary>
    public void UpdateInternals(string new_name)
    {
        internalsName = new_name;
    }
}