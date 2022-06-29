using System;

public class InteractiveNodeSpecsPanelManager : NodeSpecsPanelManager
{
    public static event Action<string> LoadInternals;
    
    public override void ChangeNode(InteractiveNode new_node)
    {
        Node = new_node;
    }

    public void EnterInternals()
    {
        LoadInternals?.Invoke(Node.GetInternalsName());
    }
}