using System;
using System.Collections.Generic;

public class NodeContents
{
    public static ExportStack RunStack = new ExportStack();

    private void AppendNewNodes(List<Action> nodes)
    {
        foreach (var node in nodes)
        {
            RunStack.AddToStack(node);
        }
    }
}


public class NodeImplementation
{
    private static List<string> _ANDGates = new List<string>();
    private static List<string> _ImplementedLoops = new List<string>();

    public static List<Action> ANDGateImpl(string name, List<string> outputs, NodeContents caller)
    {
        if (_ANDGates.Contains(name))
        {
            return ExtractOutputs(outputs, caller);
        }

        _ANDGates.Add(name);
        return new List<Action>();
    }

    public static List<Action> ProbabilityImpl(int prob, List<string> outputs, NodeContents caller)
    {
        var result = new System.Random().Next(1, 100) <= prob ? outputs : new List<string>();
        return ExtractOutputs(result, caller);
    }

    public static List<Action> DecisionImpl(int paths_taken, List<string> outputs, NodeContents caller)
    {
        if (outputs.Count <= paths_taken)
        {
            return ExtractOutputs(outputs, caller);
        }

        var chosen_outputs = new List<string>();
        var rand = new System.Random();
        for (var i = 0; i < paths_taken; i++)
        {
            var selected = outputs[rand.Next(outputs.Count)];
            outputs.Remove(selected);
            chosen_outputs.Add(selected);
        }

        return ExtractOutputs(chosen_outputs, caller);
    }

    public static List<Action> LoopImpl(string loop_name, int loop_count, List<string> loop, List<string> outputs, NodeContents caller)
    {
        // if the most recent active loop is this loop, return nothing as there's nothing new to append
        if (_ImplementedLoops.Contains(loop_name))
        {
            return new List<Action>();
        }
        _ImplementedLoops.Add(loop_name);

        var full_output = new List<string>();
        for (var i = 0; i < loop_count; i++)
        {
            full_output.AddRange(loop);
        }
        
        // remove the start of the loop to avoid infinite looping
        outputs.Remove(loop[0]);
        full_output.AddRange(outputs);
        return ExtractOutputs(full_output, caller);
    }

    public static List<Action> ExtractOutputs(List<string> method_names, NodeContents caller)
    {
        var actions = new List<Action>();

        foreach (var method_name in method_names)
        {
            actions.Add(delegate() { caller.GetType().GetMethod(method_name)?.Invoke(caller, null); });
        }

        return actions;
    }
}
