using System;

[Serializable] public class SerializedConnection
{
    public string startPos;
    public int connNodeId;
    public string endPos;
    
    /// <summary>
    /// Method <c>SerializedNode</c> initiates a serializable node from a node game object.
    /// <param name="node">The node game object to serialize.</param>
    /// </summary>
    public SerializedConnection(string start_pos, NodeConnector connector)
    {
        startPos = start_pos + "Conn";
        connNodeId = connector.GetConnectionToID();
        endPos = connector.GetConnectorToName() + "Conn";
    }
}