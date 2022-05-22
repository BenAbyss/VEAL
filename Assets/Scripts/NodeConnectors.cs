using UnityEngine;

public class NodeConnectors : MonoBehaviour
{
    [SerializeField] protected SerializeNodeConnectorsDict connectors;
    [SerializeField] protected GameObject connectedNode;

    public bool isConnecting;
    private int _nodeId;

    

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        NodeSideMenu.Connecting += BeginConnecting;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        NodeSideMenu.Connecting -= BeginConnecting;
    }
    
    /// <summary>
    /// Method <c>SetNodeId</c> sets the ID of the connectors' relevant node.
    /// <param name="node_id">The ID of the relevant node.</param>
    /// </summary>
    public void SetNodeId(int node_id)
    {
        _nodeId = node_id;
    }
    
    
    
    /// <summary>
    /// Method <c>BeginConnecting</c> sets the appropriate node up to begin connecting.
    /// <param name="node_id">The ID of the node to begin connecting.</param>
    /// </summary>
    public void BeginConnecting(int node_id)
    {
        if (node_id == _nodeId)
        {
            isConnecting = true;
            EnableConnectors();
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
    public void EnableConnectors()
    {
        foreach (var connector in connectors)
        {
            connector.Value.SetActive(!connector.Value.activeSelf);
            connectors.GetNodeFunction(connector.Key).Move(connectedNode.transform.position);
        }
    }
    
    /// <summary>
    /// Method <c>DisableConnectors</c> disables all connectors of a node.
    /// </summary>
    public void DisableConnectors()
    {
        foreach (var connector in connectors.Values)
        {
            connector.SetActive(false);
            isConnecting = false;
        }
    }
}