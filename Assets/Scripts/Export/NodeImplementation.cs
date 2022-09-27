using System.Collections.Generic;
using System.Linq;

public class NodeImplementation
{
    private static List<string> _andGates = new List<string>();

    public static List<string> ANDGateImpl(string name, List<string> outputs)
    {
        if (_andGates.Contains(name))
        {
            return outputs;
        }
        _andGates.Add(name);
        return new List<string>();
    }

    public static List<string> DecisionImpl(int paths_taken, List<string> outputs)
    {
        if (outputs.Count <= paths_taken)
        {
            return outputs;
        }

        var chosen_outputs = new List<string>();
        var rand = new System.Random();
        for (var i = 0; i < paths_taken; i++)
        {
            var selected = outputs[rand.Next(outputs.Count)];
            outputs.Remove(selected);
            chosen_outputs.Add(selected);
        }

        return chosen_outputs;
    }
}