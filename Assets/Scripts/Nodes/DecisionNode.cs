using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DecisionNode : InteractiveNode
{
    protected override string NodeType => "Decision Node";
    protected override int OutputLimit => 2;
    
    private GameObject _textContainer;
    private int _pathsTaken;
    private List<string> _pathNames;
    private List<Vector2> _distsFromNode;
    private Camera _camera;

    /// <summary>
    /// Method <c>SetPathsTaken</c> sets up all needed variables.
    /// </summary>
    public new void Start()
    {
        base.Start();
        _pathsTaken = 1;
        _pathNames = new List<string>();
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
        NodeDragged += MoveText;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
        NodeDragged -= MoveText;
    }



    /// <summary>
    /// Method <c>LabelLines</c> adds labels for each line based on their allocated name.
    /// </summary>
    public void LabelLines()
    {
        var locs = GetLineStarts();
        _distsFromNode = new List<Vector2>();
        
        foreach (Transform child in _textContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        for (var i = 0; i < Math.Min(NodeConnectors.GetOutputCount(), _pathNames.Count); i++)
        {
            var holder = Instantiate(new GameObject(), _textContainer.transform);
            var text = holder.AddComponent<TextMeshProUGUI>();
            text.fontSize = 30;
            text.color = Color.black;
            text.text = _pathNames[i];
            text.transform.position = locs[i];
            _distsFromNode.Add(_camera.ScreenToWorldPoint(locs[i]) - transform.position);
        }
    }

    /// <summary>
    /// Method <c>GetLineStarts</c> Gets the position for a name, as the edge of the lines start.
    /// </summary>
    private List<Vector2> GetLineStarts()
    {
        var locs = new List<Vector2>();
        foreach (var node in NodeConnectors.GetUsedConnectors(true, false))
        {
            if (node.GetConnectionTo().connectorGroup.nodeType == "Midpoint")
            {
                
            }
            else
            {
                locs.Add(AdjustLineStartFromConn(node));
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
            {"Btm", Vector2.down}, {"Left", Vector2.left}
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

    
    
    /// <summary>
    /// Method <c>SetPathsTaken</c> sets the number of paths taken.
    /// <param name="paths_taken">The new amount of paths to be taken by the node.</param>
    /// </summary>
    public void SetPathsTaken(int paths_taken)
    {
        _pathsTaken = paths_taken;
    }

    /// <summary>
    /// Method <c>GetPathsTaken</c> gets the number of paths taken.
    /// <returns>The number of paths taken by the node.</returns>
    /// </summary>
    public int GetPathsTaken()
    {
        return _pathsTaken;
    }

    /// <summary>
    /// Method <c>SetName</c> sets a position in the nodes' name list to a new name.
    /// <param name="new_name">The new name to add.</param>
    /// <param name="pos">The position to place the name.</param>
    /// </summary>
    public void SetName(string new_name, int pos)
    {
        while (pos > _pathNames.Count)
        {
            _pathNames.Add("");
        }
        _pathNames.Insert(pos, new_name);
        LabelLines();
    }

    /// <summary>
    /// Method <c>GetNames</c> gets the number of paths taken.
    /// <returns>The number of paths taken by the node.</returns>
    /// </summary>
    public List<string> GetNames()
    {
        return _pathNames;
    }
}