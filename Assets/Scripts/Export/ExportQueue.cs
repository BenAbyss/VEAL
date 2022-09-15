using System;
using System.Collections.Generic;

public class ExportQueue
{
    private Queue<Action> _path = new Queue<Action>();

    public void CallQueue()
    {
        while (_path.Count != 0)
        {
            _path.Dequeue()();
        }
    }

    public void AddToQueue(Action action)
    {
        _path.Enqueue(action);
    }
}