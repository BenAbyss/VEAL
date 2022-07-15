using UnityEngine;

public class ChromosomeVariable
{
    public string chrName;
    public VarType type;
    public ChromosomeLimits limits; 
    public string fitnessCalc; // wont be string
    public float fitnessWeight;

    public ChromosomeVariable(VarType new_type)
    {
        type = new_type;
        limits = new ChromosomeLimits(new_type);
    }
}