using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeConnectors : MonoBehaviour
{
    [SerializeField] protected SerializeNodeConnectorsDict connectors;
    [SerializeField] protected GameObject connectedNode;

    public bool isConnecting;
    public int nodeId;
    public string nodeType;

    private int _outputLimit;
    private int _outputCount;

    

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        NodeSideMenu.Connecting += BeginConnecting;
        NodeConnector.MakingConnection += EnableConnectors;
        NodeConnector.ConnectionRecipientMade += DisableConnectors;
        InputManager.CancelAction += DisableConnectors;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        NodeSideMenu.Connecting -= BeginConnecting;
        NodeConnector.MakingConnection -= EnableConnectors;
        NodeConnector.ConnectionRecipientMade -= DisableConnectors;
        InputManager.CancelAction -= DisableConnectors;
    }

    /// <summary>
    /// Method <c>Start</c> passes this object down to all individual connectors.
    /// </summary>
    public void Start()
    {
        foreach (var connector in connectors)
        {
            connectors.GetNodeFunction(connector.Key).connectorGroup = this;
        }

        _outputCount = 0;
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
    /// Method <c>GetNodeDepth</c> returns the depth of the relevant node.
    /// <returns>The nodes' Z coordinate depth.</returns>
    /// </summary>
    public float GetNodeDepth()
    {
        return connectedNode.transform.position.z;
    }
    
    /// <summary>
    /// Method <c>SetNodeType</c> sets the class type of the node.
    /// <param name="node_type">The type of the relevant node.</param>
    /// </summary>
    public void SetNodeType(string node_type)
    {
        nodeType = node_type;
    }
    
    /// <summary>
    /// Method <c>SetOutputLimit</c> sets the amount of possible outputs.
    /// <param name="output_limit">The maximum amount of outputs allowed.</param>
    /// </summary>
    public void SetOutputLimit(int output_limit)
    {
        _outputLimit = output_limit;
    }
    
    /// <summary>
    /// Method <c>GetOutputLimitReached</c> states whether the output limit has been reached.
    /// <returns>A boolean stating whether the limit has been reached</returns>
    /// </summary>
    public bool OutputLimitReached()
    {
        return _outputLimit <= _outputCount;
    }
    
    
    
    /// <summary>
    /// Method <c>IncrementOutputCount</c> increments the output counter by 1.
    /// </summary>
    public void IncrementOutputCount()
    {
        _outputCount += 1;
    }
    
    /// <summary>
    /// Method <c>DecrementOutputCount</c> decrements the output counter by 1.
    /// </summary>
    public void DecrementOutputCount()
    {
        _outputCount -= 1;
    }
    
    /// <summary>
    /// Method <c>BeginConnecting</c> sets the appropriate node up to begin connecting.
    /// <param name="node_id">The ID of the node to begin connecting.</param>
    /// </summary>
    public void BeginConnecting(int node_id)
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
    /// Method <c>CheckForConnectedNode</c> checks whether a node already has a connection to or from this node.
    /// <param name="node_id">The ID of the node to check for connections for.</param>
    /// <returns>A boolean stating if there's a connection.</returns>
    /// </summary>
    public bool CheckForConnectedNode(int node_id)
    {
        foreach (var connector in connectors)
        {
            var function = connectors.GetNodeFunction(connector.Key);
            if (function.GetConnectionFromID() == node_id || function.GetConnectionToID() == node_id)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Method <c>GetUsedConnectors</c> gets all connectors that are outputting and/or inputting a connection.
    /// <returns>Connectors with a connection.</returns>
    /// </summary>
    public List<NodeConnector> GetUsedConnectors(bool outputs = true, bool inputs = true)
    {
        List<NodeConnector> conns = new List<NodeConnector>();
        foreach (var connector in connectors)
        {
            var function = connectors.GetNodeFunction(connector.Key);
            if (outputs && function.HasOutput() || inputs && function.HasInput())
            {
                conns.Add(function);
            }
        }
        return conns;
    }
    
    
    
    /// <summary>
    /// Method <c>EnableConnectors</c> enables all connectors of a node.
    /// </summary>
    public void EnableConnectors()
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
            if (function.GetConnectingNode())
            {
                function.SetConnectingNode(false);
                function.RemoveDrawings();
            }
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