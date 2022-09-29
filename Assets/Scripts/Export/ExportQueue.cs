using System;
using System.Collections.Generic;

public class ExportQueue
{
    private static string _LastCall;
    private Queue<(Action, string)> _path = new Queue<(Action, string)>();

    public void CallQueue()
    {
        while (_path.Count != 0)
        {
            var next_call = _path.Dequeue();
            next_call.Item1();
            _LastCall = next_call.Item2;
        }
    }

    public void AddToQueue(Action action, string name)
    {
        _path.Enqueue((action, name));
    }

    public static string GetLastCall()
    {
        return _LastCall;
    }
}