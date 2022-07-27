using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable] 
public class ChromosomeLimits
{
    public VarType LimitType;
    public Dictionary<string, int> NumVal;
    public Dictionary<string, int> StrLength;
    public string Equation;
    public int DecPlaces;
    public List<string> InvalidStrings;
    public List<(string, float)> EnumOptions;

    /// <summary>
    /// Method <c>ChromosomeLimits</c> sets up the limits.
    /// <param name="limit">The type of the limit.</param>
    /// <param name="num_val">The max and/or min of the number, if appropriate.</param>
    /// <param name="str_length">The max and/or min of the string length, if appropriate.</param>
    /// <param name="eq">The equation for the input to meet.</param>
    /// <param name="dec_places">The amount of decimal places to apply it to.</param>
    /// <param name="invalid_strings">A list of any invalid strings.</param>
    /// </summary>
    public ChromosomeLimits(VarType limit, Dictionary<string, int> num_val=null, 
        Dictionary<string, int> str_length=null, string eq=null, int dec_places=-1, List<string> invalid_strings=null,
        List<(string, float)> enum_options=null)
    {
        LimitType = limit;
        NumVal = num_val;
        StrLength = str_length;
        Equation = eq;
        DecPlaces = dec_places;
        InvalidStrings = invalid_strings;
        EnumOptions = enum_options;
    }
}