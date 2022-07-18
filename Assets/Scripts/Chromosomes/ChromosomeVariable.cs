using System;
using UnityEngine;

[Serializable] public class ChromosomeVariable
{
    public string chrName;
    public VarType type;
    public ChromosomeLimits limits;
    public FitnessType fitnessCalc;
    public float fitnessTargetVal; // wont be string
    public float fitnessWeight;

    public ChromosomeVariable(VarType new_type)
    {
        type = new_type;
        limits = new ChromosomeLimits(new_type);
    }

    public void SetupFitness(FitnessType fitness_type, float target, float weight)
    {
        fitnessCalc = fitness_type;
        fitnessTargetVal = target;
        fitnessWeight = weight;
    }
}