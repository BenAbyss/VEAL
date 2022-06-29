using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SceneSave : NodeSave
{
    protected override string Subdirectory => "Scenes";
    public static event Action<string> SceneChanged;
    
    private string _currentScene = "MainScene"; // replaced by save file name if it's a saved project
    private Stack<string> _previousScenes = new Stack<string>();
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        base.OnEnable();
        InteractiveNodeSpecsPanelManager.LoadInternals += SwapScenes;
        InternalsMenuManager.PreviousScene += PreviousScene;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        base.OnDisable();
        InteractiveNodeSpecsPanelManager.LoadInternals -= SwapScenes;
        InternalsMenuManager.PreviousScene -= PreviousScene;
    }



    /// <summary>
    /// Method <c>SwapScenes</c> saves and destroys the current scene, before loading the new scene.
    /// <param name="filename">The name of the new scene to load.</param>
    /// </summary>
    private void SwapScenes(string filename)
    {
        SaveNodes(_currentScene);
        // destroy all currently active nodes
        foreach (var node in FindObjectsOfType<BasicNode>())
        {
            var full_obj = node.transform.parent;
            if (node.GetNodeType() != "Midpoint")
            {
                full_obj = full_obj.parent;
            }
            Destroy(full_obj.gameObject);
        }

        _previousScenes.Push(_currentScene);
        _currentScene = filename;
        LoadNodes(_currentScene);
        SceneChanged?.Invoke(filename);
    }

    /// <summary>
    /// Method <c>PreviousScene</c> saves and destroys the current scene, before loading the previous scene.
    /// </summary>
    private void PreviousScene()
    {
        SwapScenes(_previousScenes.Pop());
    }
    
    
    
    /// <summary>
    /// Method <c>SaveNodes</c> serializes and saves all currently active nodes.
    /// <param name="filename">The name of the file to store the serialization within.</param>
    /// </summary>
    private void SaveNodes(string filename)
    {
        var nodes = FindObjectsOfType<BasicNode>();
        var json_serialized = nodes.Select(node => SerializeNode(node.gameObject)).ToList();

        File.WriteAllLines(NewFilePath(filename), json_serialized);
    }

    /// <summary>
    /// Method <c>LoadNodes</c> loads and instantiates nodes from serialization.
    /// <param name="filename">The name of the file to extract the serialization from.</param>
    /// </summary>
    private void LoadNodes(string filename)
    {
        filename = ToPath(filename);
        if (!File.Exists(filename)) return;
        var contents = File.ReadLines(filename).ToList();

        var nodes = contents.Select(serialized_node => LoadNode(serialized_node)).ToList();
        LoadConnections(contents, nodes);
    }

    private static void LoadConnections(IEnumerable<string> file_lines, List<GameObject> nodes)
    {
        var node_ids = nodes.Select(node => node.GetComponentInChildren<BasicNode>().nodeId).ToList();
        
        foreach (var node_json in file_lines.Select(line => JsonUtility.FromJson<SerializedNode>(line)))
        {
            if (node_json.connections == null) continue;
            
            // for each node with connections
            var node = nodes.First(n => 
                n.GetComponentInChildren<BasicNode>().nodeId == node_json.nodeId);

            foreach (var conn in node_json.connections)
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