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

    public static List<string> LoopImpl(int loop_count, List<string> loop, List<string> outputs)
    {
        // if the last called node was the end of this loop, return nothing
        // as there's nothing new to append
        if (ExportQueue.GetLastCall() == loop[loop.Count - 1])
        {
            return new List<string>();
        }
        
        var full_output = new List<string>();
        for (var i = 0; i < loop_count; i++)
        {
            full_output.AddRange(loop);
        }
        
        // remove the start of the loop to avoid infinite looping
        outputs.Remove(loop[0]);
        full_output.AddRange(outputs);
        return full_output;
    }
    
}