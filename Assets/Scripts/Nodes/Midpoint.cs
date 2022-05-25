using UnityEngine;

public class Midpoint : BasicNode
{
    public NodeConnector GetConnector(string pos)
    {
        var f = transform.parent;
        var g = transform.parent.Find(pos);
        return transform.parent.Find(pos).GetComponent<NodeConnector>();
    }

    public void ContinueConnection()
    {
        NodeConnectors.EnableConnectors();
    }
}