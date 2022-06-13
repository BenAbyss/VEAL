using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using DebugAssert = System.Diagnostics.Debug;

public class Midpoint : BasicNode, IEndDragHandler, IPointerDownHandler
{
    protected override string NodeType => "Midpoint";
    private string _autoConnPos;
    private List<BasicNode> _parentNodes = new List<BasicNode>();
    
    /// <summary>
    /// Method <c>Awake</c> sets the node counter & finds relevant components.
    /// </summary>
    public new void Awake()
    {
        NodesCounter += 1;
        nodeId = NodesCounter;
        NodeConnectors = GetComponent<NodeConnectors>();
        NodeConnectors.SetNodeId(nodeId);
        NodeConnectors.SetOutputLimit(OutputLimit);
        
        NodeConnectors.SetupConnectors(transform.position);
        NodeConnectors.SetNodeType(NodeType);
        NodeConnectors.SetOutputLimit(OutputLimit);
    }
    
    /// <summary>
    /// Method <c>Start</c> nulls the start function.
    /// </summary>
    public new void Start()
    {
        
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        NodeSelected += NewNodeSelected;
        NodeConnectors.OutputCountChanged += AdjustParentOutputCount;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
        NodeConnectors.OutputCountChanged -= AdjustParentOutputCount;
    }
    
    
    
    
    /// <summary>
    /// Method <c>OnPointerUp</c> marks the node as selected and reveals the side menu as the node is placed.
    /// </summary>
    public new void OnEndDrag(PointerEventData event_data)
    {
        try
        {
            NodeConnectors.BeginConnecting(nodeId);
            base.OnEndDrag(event_data);
        } catch(NullReferenceException) {}
    }

    /// <summary>
    /// Method <c>OnPointerDown</c> marks the node as selected and reveals the side menu.
    /// </summary>
    public new void OnPointerDown(PointerEventData event_data)
    {
        try
        {
            NodeConnectors.BeginConnecting(nodeId);
            base.OnPointerDown(event_data);
        } catch(NullReferenceException) {}
    }



    /// <summary>
    /// Method <c>AddParent</c> adds a 'parent' node that connects to the midpoint, and increments it's outputs
    /// accordingly.
    /// <param name="parent">The parent node.</param>
    /// </summary>
    public void AddParent(BasicNode parent)
    {
        _parentNodes.Add(parent);
        NodeConnectors.SetOutputLimit(
            Math.Min(OutputLimit, parent.GetOutputLimit() - parent.GetNodeConnectors().GetOutputCount()));
        // this includes a -1 to counter the midpoint being considered an output by the node itself
        parent.GetNodeConnectors().IncrementOutputCount(NodeConnectors.GetOutputCount() - 1);
    }
    
    /// <summary>
    /// Method <c>RemoveParent</c> removes a 'parent' node that connects to the midpoint, and decrements it's outputs
    /// accordingly.
    /// <param name="parent">The parent node.</param>
    /// </summary>
    public void RemoveParent(BasicNode parent)
    {
        _parentNodes.Remove(parent);
        parent.GetNodeConnectors().DecrementOutputCount(NodeConnectors.GetOutputCount());

        NodeConnectors.SetOutputLimit(_parentNodes.Min(
                                          par => par.GetOutputLimit() - par.GetNodeConnectors().GetOutputCount())
                                      + NodeConnectors.GetOutputCount());
    }

    /// <summary>
    /// Method <c>AdjustParentOutputCount</c> adjusts the output count of parents when the midpoint makes a new
    /// connection, and lowers the midpoints output limit when a parent makes a connection.
    /// <param name="node_id">The id of the node adjusting.</param>
    /// <param name="val">The amount it's been adjusted by.</param>
    /// </summary>
    private void AdjustParentOutputCount(int node_id, int val)
    {
        if (_parentNodes.Select(parent => parent.nodeId).Contains(node_id))
        {
            NodeConnectors.SetOutputLimit(NodeConnectors.GetOutputLimit() - val);
        }
        else if (node_id == nodeId)
        {
            foreach (var parent in _parentNodes)
            {
                parent.GetNodeConnectors().IncrementOutputCount(val, false);
            }
        }
        
    }



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