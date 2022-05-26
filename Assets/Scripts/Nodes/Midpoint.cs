using System.Collections.Generic;
using System.Linq;
using DebugAssert = System.Diagnostics.Debug;

public class Midpoint : BasicNode
{
    protected override string NodeType => "Midpoint";
    private string _autoConnPos;
    
    /// <summary>
    /// Method <c>GetConnector</c> gets the connector in a given position.
    /// <param name="pos">The position of the connector - Top, Right, Left or Btm.</param>
    /// <returns>The given connector object.</returns>
    /// </summary>
    public NodeConnector GetConnector(string pos)
    {
        _autoConnPos = pos;
        return transform.parent.Find(pos).GetComponent<NodeConnector>();
    }

    /// <summary>
    /// Method <c>ContinueConnection</c> sets the node to begin connecting from the opposite node to that which
    /// accepted the connection.
    /// </summary>
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