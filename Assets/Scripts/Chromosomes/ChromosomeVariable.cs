using UnityEngine;

public class ChromosomeVariable : MonoBehaviour
{
    public enum VarType
    {
        Integer, Float, Boolean,
        String, Character,
        Enumerator, Struct
    }
    
    public string chrName;
    public VarType type;
    public string limits; // wont be string
    public string fitnessCalc; // wont be string
    public float fitnessWeight;
}