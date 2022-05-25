using DebugAssert = System.Diagnostics.Debug;

public class Midpoint : BasicNode
{
    private string _autoConnPos;
    
    public NodeConnector GetConnector(string pos)
    {
        _autoConnPos = pos;
        return transform.parent.Find(pos).GetComponent<NodeConnector>();
    }

    public void ContinueConnection()
    {
        NodeConnectors.EnableConnectors();
        NodeConnectors.isConnecting = true;
        
        var opposite_pos = "";
        switch (_autoConnPos)
        {
            case "TopConn":   opposite_pos = "BtmConn";   break;
            case "BtmConn":   opposite_pos = "TopConn";   break;
            case "LeftConn":  opposite_pos = "RightConn"; break;
            case "RightConn": opposite_pos = "LeftConn";  break;
        }
        GetConnector(opposite_pos).Clicked();
    }
}