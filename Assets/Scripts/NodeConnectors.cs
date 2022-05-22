using System;
using System.Linq;
using UnityEngine;

public class NodeConnectors : MonoBehaviour
{
    [SerializeField] protected SerializeNodeConnectorsDict connectors;
    [SerializeField] protected GameObject connectedNode;

    public bool isConnecting;
    public int nodeId;

    

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        NodeSideMenu.Connecting += BeginConnecting;
        NodeConnector.MakingConnection += EnableConnectors;
        NodeConnector.ConnectionRecipientMade += DisableConnectors;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        NodeSideMenu.Connecting -= BeginConnecting;
        NodeConnector.MakingConnection -= EnableConnectors;
        NodeConnector.ConnectionRecipientMade -= DisableConnectors;
    }

    /// <summary>
    /// Method <c>Start</c> passes this object down to all individual connectors.
    /// </summary>
    public void Start()
    {
        foreach (var connector in connectors)
        {
            connectors.GetNodeFunction(connector.Key).SetConnectorGroup(this);
        }
    }

    /// <summary>
    /// Method <c>SetNodeId</c> sets the ID of the connectors' relevant node.
    /// <param name="node_id">The ID of the relevant node.</param>
    /// </summary>
    public void SetNodeId(int node_id)
    {
        nodeId = node_id;
    }
    
    
    
    /// <summary>
    /// Method <c>BeginConnecting</c> sets the appropriate node up to begin connecting.
    /// <param name="node_id">The ID of the node to begin connecting.</param>
    /// </summary>
    private void BeginConnecting(int node_id)
    {
        if (node_id == nodeId)
        {
            isConnecting = !isConnecting;
            ToggleConnectors();
        }
    }
    
    /// <summary>
    /// Method <c>SetupConnectors</c> sets up a nodes' connector positions.
    /// <param name="node_pos">The position of the node.</param>
    /// </summary>
    public void SetupConnectors(Vector3 node_pos)
    {
        isConnecting = false;
        foreach (var connector_function in connectors.GetNodeFunctions())
        {
            connector_function.SetPositioning(node_pos);
        }
        DisableConnectors();
    }
    
    
    
    /// <summary>
    /// Method <c>EnableConnectors</c> enables all connectors of a node.
    /// </summary>
    private void EnableConnectors()
    {
        foreach (var function in 
            connectors.Select(connector => connectors.GetNodeFunction(connector.Key)))
        {
            function.ChangeVisibility(true);
            function.Move(connectedNode.transform.position);
        }
    }
    
    /// <summary>
    /// Method <c>ToggleConnectors</c> toggles all connectors of a node.
    /// </summary>
    private void ToggleConnectors()
    {
        foreach (var function in 
            connectors.Select(connector => connectors.GetNodeFunction(connector.Key)))
        {
            function.ChangeVisibility(!function.isVisible);
            function.Move(connectedNode.transform.position);
        }
    }
    
    /// <summary>
    /// Method <c>DisableConnectors</c> disables all connectors of a node.
    /// </summary>
    public void DisableConnectors()
    {
        foreach (var function in 
            connectors.Select(connector => connectors.GetNodeFunction(connector.Key)))
        {
            function.ChangeVisibility(false);
            isConnecting = false;
        }
    }

    /// <summary>
    /// Method <c>DisableConnectors</c> disables all connectors of a node.
    /// <param name="recipient">The recipient node of the connection.</param>
    /// </summary>
    private void DisableConnectors(NodeConnector recipient)
    {
        foreach (var function in 
            connectors.Select(connector => connectors.GetNodeFunction(connector.Key)))
        {
            function.ChangeVisibility(false);
            isConnecting = false;
        }
    }
}