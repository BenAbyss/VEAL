using System.Collections.Generic;
using System.Linq;

public class NodeImplementation
{
    private static List<string> _andGates = new List<string>();

    public static bool ANDGateImpl(string name)
    {
        if (_andGates.Contains(name))
        {
            return true;
        }
        _andGates.Add(name);
        return false;
    }
}