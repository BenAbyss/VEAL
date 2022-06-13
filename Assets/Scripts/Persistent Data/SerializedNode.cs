using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable] public class SerializedNode
{
    public int nodeId;
    public string nodeType;
    public string nodeName;
    public Vector3 position;
    public Color colour;
    public List<SerializedConnection> Connections;

    /// <summary>
    /// Method <c>SerializedNode</c> initiates a serializable node from a node game object.
    /// <param name="node">The node game object to serialize.</param>
    /// </summary>
    public SerializedNode(GameObject node)
    {
        SerializeNodeData(node.GetComponentInChildren<BasicNode>());
        SerializeConnData(node.GetComponentInChildren<NodeConnectors>());

        position = node.transform.position;
        colour = node.transform.GetComponent<SpriteRenderer>().color;
    }

    /// <summary>
    /// Method <c>SerializeNodeData</c> gets all needed data from the node.
    /// <param name="data">The node data.</param>
    /// </summary>
    private void SerializeNodeData(BasicNode data)
    {
        nodeId = data.nodeId;
        nodeType = data.GetNodeType();
        nodeName = data.name;
    }

    /// <summary>
    /// Method <c>SerializeConnData</c> gets all needed data from the node connectors.
    /// <param name="data">The connector group data.</param>
    /// </summary>
    private void SerializeConnData(NodeConnectors data)
    {
        Connections = new List<SerializedConnection>();
        foreach (var conn in data.GetUsedConnectorsFull(true, false))
        {
            Connections.Add(new SerializedConnection(conn.Key, conn.Value.GetComponent<NodeConnector>()));
        }
    }
}