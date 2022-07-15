using System.Collections.Generic;
using UnityEngine;

public class ChromosomeLimits
{
    public VarType LimitType;
    public int[] NumVal;
    public int[] StrLength;
    public string Equation;
    public int DecPlaces;
    public List<string> InvalidStrings;

    public ChromosomeLimits(VarType limit, int[] num_val=null, int[] str_length=null, string eq=null, 
        int dec_places=-1, List<string> invalid_strings=null)
    {
        LimitType = limit;
        NumVal = num_val;
        StrLength = str_length;
        Equation = eq;
        DecPlaces = dec_places;
        InvalidStrings = invalid_strings;
    }
}