using System;
using System.Collections.Generic;

public class ExportStack
{
    private Stack<Action> _path = new Stack<Action>();

    public void CallStack()
    {
        while (_path.Count != 0)
        {
            _path.Pop()();
        }
    }

    public void AddToStack(Action action)
    {
        _path.Push(action);
    }
}