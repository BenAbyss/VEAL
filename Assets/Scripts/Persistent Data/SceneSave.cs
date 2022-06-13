using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SceneSave : SaveSystem
{
    protected override string Subdirectory => "Scenes";

    public void SaveAndDelete()
    {
        SaveNodes("testing");
        foreach (var node in FindObjectsOfType<BasicNode>())
        {
            var full_obj = node.transform.parent;
            if (node.GetNodeType() != "Midpoint")
            {
                full_obj = full_obj.parent;
            }
            Destroy(full_obj.gameObject);
        }
    }

    public void Load()
    {
        LoadNodes("testing");
    }
    
    /// <summary>
    /// Method <c>SaveNodes</c> serializes and saves all currently active nodes.
    /// <param name="filename">The name of the file to store the serialization within.</param>
    /// </summary>
    private void SaveNodes(string filename)
    {
        var nodes = FindObjectsOfType<BasicNode>();
        List<SerializedNode> serialized_nodes = 
            (from node in nodes where node.enabled select new SerializedNode(node.gameObject)).ToList();
        List<string> json_serialized = (from node in serialized_nodes select JsonUtility.ToJson(node)).ToList();
        
        File.WriteAllLines(ValidName(filename), json_serialized);
    }

    /// <summary>
    /// Method <c>LoadNodes</c> loads and instantiates nodes from serialization.
    /// <param name="filename">The name of the file to extract the serialization from.</param>
    /// </summary>
    private void LoadNodes(string filename)
    {
        if (!File.Exists(filename)) return;
        List<GameObject> nodes = new List<GameObject>();
        var contents = File.ReadLines(filename).ToList();
        var remove_whitespace = new Regex(@"\s+");
        
        foreach (var serialized_node in contents)
        {
            var node_json = JsonUtility.FromJson<SerializedNode>(serialized_node);
            var prefab_name = "Prefabs/Nodes/" +
                              $"{remove_whitespace.Replace(node_json.nodeType, "")}";
            var node = Instantiate(Resources.Load(prefab_name), 
                GameObject.Find("MainCanvas").transform) as GameObject;
            nodes.Add(node);
            var node_obj = node.GetComponentInChildren<BasicNode>();
            
            node.transform.position = node_json.position;
            node_obj.SetNodeId(node_json.nodeId);
            node_obj.SetNodeName(node_json.nodeName);
            var sprite_renderer = node_json.nodeType != "Midpoint" ? 
                node.transform.Find("ConnectableNode").Find("MainNode").GetComponent<SpriteRenderer>() : 
                node.transform.Find("Midpoint").GetComponent<SpriteRenderer>();
            sprite_renderer.color = node_json.colour;
        }
        LoadConnections(contents, nodes);
    }

    private void LoadConnections(IEnumerable<string> file_lines, List<GameObject> nodes)
    {
        var node_ids = nodes.Select(node => node.GetComponentInChildren<BasicNode>().nodeId).ToList();
        
        foreach (var node_json in file_lines.Select(line => JsonUtility.FromJson<SerializedNode>(line)))
        {
            if (node_json.Connections == null) continue;
            
            // for each node with connections
            var node = nodes.First(n => 
                n.GetComponentInChildren<BasicNode>().nodeId == node_json.nodeId);

            foreach (var conn in node_json.Connections)
            {
                // form a connection matching the data
                var conn_node = nodes[node_ids.IndexOf(conn.connNodeId)];
                
                if (node.GetComponentInChildren<BasicNode>().GetNodeType() != "Midpoint")
                {
                    node = node.transform.Find("ConnectableNode").gameObject;
                }
                if (conn_node.GetComponentInChildren<BasicNode>().GetNodeType() != "Midpoint")
                {
                    conn_node = conn_node.transform.Find("ConnectableNode").gameObject;
                }
                
                node.transform.Find(conn.startPos).GetComponent<NodeConnector>().FormLoadedConnection(
                    conn_node.transform.Find(conn.endPos).GetComponent<NodeConnector>(), 
                    node.GetComponentInChildren<NodeConnectors>(), 
                    conn_node.GetComponentInChildren<NodeConnectors>());
            }
        }
    }
}