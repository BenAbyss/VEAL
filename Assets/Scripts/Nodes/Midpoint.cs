using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using DebugAssert = System.Diagnostics.Debug;

public class Midpoint : BasicNode, IEndDragHandler, IPointerDownHandler
{
    protected override string NodeType => "Midpoint";
    private string _autoConnPos;
    private List<BasicNode> _parentNodes = new List<BasicNode>();
    
    private GameObject _textContainer;
    private List<Vector2> _distsFromNode;
    private Camera _camera;
    private int _pathsTaken;

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
        _distsFromNode = new List<Vector2>();
        _textContainer = transform.parent.Find("Text").gameObject;
        _camera = Camera.main;
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        NodeSelected += NewNodeSelected;
        NodeConnectors.OutputCountChanged += AdjustParentOutputCount;
        NodeDragged += MoveText;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
        NodeConnectors.OutputCountChanged -= AdjustParentOutputCount;
        NodeDragged -= MoveText;
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



    public void LabelLine(Vector2 pos, string path_name)
    {
        var holder = Instantiate(new GameObject(), _textContainer.transform);
        var text = holder.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        text.fontSize = 30;
        text.color = Color.white;
        text.text = path_name;
        text.transform.position = pos;
        _distsFromNode.Add(_camera.ScreenToWorldPoint(pos) - transform.position);
    }
    
    /// <summary>
    /// Method <c>GetLineStarts</c> Gets the position for a name, as the edge of the lines start.
    /// </summary>
    public List<(Vector2, Midpoint)> GetLineStarts(List<string> names)
    {
        var locs = new List<(Vector2, Midpoint)>();
        var nodes = NodeConnectors.GetUsedConnectors(true, false);
        for (var i = 0; i < NodeConnectors.GetOutputCount(); i++)
        {
            if (nodes[i].GetConnectionTo().connectorGroup.nodeType == "Midpoint")
            {
                locs[i].Item2.LabelLine(locs[i].Item1, names[i]);
            }
            else
            {
                locs.Add((AdjustLineStartFromConn(nodes[i]), this));
            }
        }

        return locs;
    }
    
    /// <summary>
    /// Method <c>AdjustLineStartFromConn</c> adjusts the position for a lines text based on it's node position.
    /// <param name="conn">The node connector for the line.</param>
    /// </summary>
    private Vector2 AdjustLineStartFromConn(NodeConnector conn)
    {
        Vector2 pos = conn.transform.position;
        var adjust = new Dictionary<string, Vector2>()
        {
            {"Top", Vector2.up}, {"Right", Vector2.right},
            {"Btm", Vector2.down*2}, {"Left", Vector2.left}
        };
        var adjust_outcome = adjust[conn.name.Replace("Conn", "")];
        pos += adjust_outcome * 5 + new Vector2(-1*adjust_outcome.y, -1*adjust_outcome.x) * 2;
        
        return _camera.WorldToScreenPoint(pos);
    }
    
    /// <summary>
    /// Method <c>MoveText</c> moves the line texts with the node.
    /// <param name="node_id">The ID of the moving node.</param>
    /// <param name="dist">The distance the node has moved.</param>
    /// <param name="is_connecting">Whether the node is currently connecting.</param>
    /// </summary>
    private void MoveText(int node_id, Vector3 dist, bool is_connecting=false)
    {
        if (node_id != nodeId) return;
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        var counter = 0;
        foreach (RectTransform text in _textContainer.transform)
        {
            text.position = _camera.WorldToScreenPoint((Vector2)dist + _distsFromNode[counter]);
            text.sizeDelta =
                _camera.orthographicSize * transform.lossyScale;
            counter++;
        }
    }
}