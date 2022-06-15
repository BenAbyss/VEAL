using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static event Action<Midpoint> MidpointCreated;
    public static event Action<GameObject, string> SaveNode;
    [SerializeField] private GameObject midpointPrefab;
    private GameObject _selectedNode;

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        BasicNode.NodeSelected += SetSelectedNode;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        BasicNode.NodeSelected -= SetSelectedNode;
    }
    
    /// <summary>
    /// Method <c>SetSelectedNode</c> destroys the currently selected node.
    /// <returns>The selected node game object</returns>
    /// </summary>
    public void DestroySelectedNode()
    {
        if (_selectedNode == null) return;
        
        var node_func = _selectedNode.GetComponent<BasicNode>();
        foreach (var conn in node_func.GetNodeConnectors().GetUsedConnectors())
        {
            conn.ClearConnections();
            Destroy(conn);
        }

        var full_obj = _selectedNode.transform.parent;
        if (node_func.GetNodeType() != "Midpoint")
        {
            full_obj = full_obj.parent;
        }
        Destroy(full_obj.gameObject);
    }
    
    /// <summary>
    /// Method <c>SetSelectedNode</c> sets the current selected node.
    /// <param name="node">The selected node game object.</param>
    /// </summary>
    private void SetSelectedNode(int id_selected, Vector3 new_pos, GameObject node)
    {
        _selectedNode = node;
    }



    /// <summary>
    /// Method <c>SaveSelectedNode</c> saves the currently selected node.
    /// </summary>
    public void SaveSelectedNode()
    {
        SaveNode?.Invoke(_selectedNode, _selectedNode.GetComponent<BasicNode>().name);
    }



    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Method <c>CreateMidpoint</c> instantiates a midpoint. 
    /// </summary>
    /// This is done as an IEnumerator to allow for a brief delay to let the midpoint set up.
    public IEnumerator CreateMidpoint()
    {
        var coords = InputManager.GetMouseCoords();
        coords.z = -1;
        var midpoint = Instantiate(midpointPrefab, coords, Quaternion.identity,
            GameObject.FindGameObjectWithTag("Canvas").transform);
        yield return new WaitForSeconds(0.001F);
        var midpoint_func = midpoint.GetComponentInChildren<Midpoint>();
        midpoint.name = "Midpoint " + midpoint_func.nodeId;
        MidpointCreated?.Invoke(midpoint_func);
        midpoint_func.ContinueConnection();
    }

    /// <summary>
    /// Method <c>MidpointPathHitsNode</c> checks if a given connectors' group has a midpoint path to a node.
    /// If this connector is a midpoints', it traverses to the midpoints node origin also.
    /// <param name="node_id">The id of the node to check for connections with.</param>
    /// <param name="start_conn">The connector to begin checks from.</param>
    /// <returns>A boolean stating whether it has a path to the node.</returns>
    /// </summary>
    public static bool MidpointPathHitsNode(int node_id, NodeConnector start_conn)
    {
        var nodes_hit = new List<int>();
        foreach (var conn in start_conn.connectorGroup.GetUsedConnectors(true, false))
        {
            nodes_hit = nodes_hit.Concat(FollowMidpointPath(conn)).ToList();
        }

        foreach (var origin_group_conn in start_conn.connectorGroup.GetUsedConnectors(false))
        {
            var origin = FindMidpointPathOrigin(origin_group_conn);
            if (origin == null) continue;
            foreach (var conn in origin.GetUsedConnectors(true, false))
            {
                nodes_hit = nodes_hit.Concat(FollowMidpointPath(conn)).ToList();
            }
        }

        return nodes_hit.Contains(node_id);
    }

    /// <summary>
    /// Method <c>FindMidpointPathOrigin</c> find the node origin of a midpoint.
    /// <param name="conn">A connector of the midpoint to traverse from.</param>
    /// <returns>The connector group the midpoint path originates from.</returns>
    /// </summary>
    private static NodeConnectors FindMidpointPathOrigin(NodeConnector conn)
    {
        while (true)
        {
            var node_conn = conn.GetConnectionFrom();
            if (node_conn == null) return null;
            var midpt = node_conn.connectorGroup;
            if (midpt.nodeType != "Midpoint") return midpt;
            conn = node_conn;
        }
    }

    /// <summary>
    /// Method <c>FollowMidpointPath</c> follows the midpoint path onwards, noting all nodes hit from possible paths.
    /// <param name="conn">A connector of the midpoint to traverse from.</param>
    /// <param name="nodes">The id's of nodes already hit.</param>
    /// <returns>A list of the id's of nodes hit.</returns>
    /// </summary>
    private static List<int> FollowMidpointPath(NodeConnector conn, List<int> nodes=default)
    {
        if (nodes == default)
        {
            nodes = new List<int>();
        }
        var node_conn = conn.GetConnectionTo();

        if (node_conn != null)
        {
            var midpt = node_conn.connectorGroup;
            if (midpt.nodeType == "Midpoint")
            {
                foreach (var output in midpt.GetUsedConnectors(true, false))
                {
                    nodes = FollowMidpointPath(output, nodes);
                }
            }
            else
            {
                nodes.Add(midpt.nodeId);
            }
        }
        return nodes;
    }
}