﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SceneSave : NodeSave
{
    protected override string Subdirectory => "Scenes";
    public static event Action<string, bool> SceneChanged;
    public static event Action ChromosomeEntered;
    private static string _ChromosomePath;
    
    private string _currentScene = "NodesScene"; // replaced by save file name if it's a saved project
    private Stack<string> _previousScenes = new Stack<string>();

    public void Awake()
    {
        // if a chromosome's evolutionary action is being made, swap scenes to that path
        if (_ChromosomePath != null && MenuManager.ItemCreating != "Nodes" && MenuManager.ItemCreating != "Chromosome")
        {
            SwapScenes(_ChromosomePath, false);
        }
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        base.OnEnable();
        InteractiveNodeSpecsPanelManager.LoadInternals += LoadInternals;
        ExportManager.LoadNodesScene += LoadNodesScene;
        InternalsMenuManager.PreviousScene += PreviousScene;
        MutationDataPiece.LoadInternals += LoadChromosomeInternals;
        CrossoverDataPiece.LoadInternals += LoadChromosomeInternals;
        ExportManager.LoadInternals += LoadInternals;
        InternalsMenuManager.ReturnToChromosome += () => Save(_ChromosomePath);
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        base.OnDisable();
        InteractiveNodeSpecsPanelManager.LoadInternals -= LoadInternals;
        ExportManager.LoadNodesScene -= LoadNodesScene;
        InternalsMenuManager.PreviousScene -= PreviousScene;
        MutationDataPiece.LoadInternals -= LoadChromosomeInternals;
        CrossoverDataPiece.LoadInternals -= LoadChromosomeInternals;
        ExportManager.LoadInternals -= LoadInternals;
    }

    
    
    /// <summary>
    /// Method <c>LoadInternals</c> saves and destroys the current scene, before loading the main nodes scene.
    /// </summary>
    private void LoadNodesScene()
    {
        SwapScenes("NodesScene", true);
    }
    
    /// <summary>
    /// Method <c>LoadInternals</c> saves and destroys the current scene, before loading the new scene.
    /// <param name="filename">The name of the new scene to load.</param>
    /// </summary>
    private void LoadInternals(string filename)
    {
        SwapScenes(filename, true);
    }

    private void LoadChromosomeInternals(string internals_name, string chromosome_name, string type)
    {
        _ChromosomePath = ToFolderPath(chromosome_name, internals_name);
        MenuManager.ItemCreating = internals_name;
        ChromosomeEntered?.Invoke();
    }


    
    /// <summary>
    /// Method <c>SwapScenes</c> saves and destroys the current scene, before loading the new scene.
    /// <param name="filename">The name of the new scene to load.</param>
    /// <param name="add_scene">Whether to add this new scene to the stack of scenes.</param>
    /// </summary>
    private void SwapScenes(string filename, bool add_scene)
    {
        Save(_currentScene);
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

        if (add_scene)
        {
            _previousScenes.Push(_currentScene);
        }
        _currentScene = filename;
        LoadNodes(_currentScene);
        SceneChanged?.Invoke(filename, _previousScenes.Count != 0);
    }

    /// <summary>
    /// Method <c>PreviousScene</c> saves and destroys the current scene, before loading the previous scene.
    /// </summary>
    private void PreviousScene()
    {
        SwapScenes(_previousScenes.Pop(), false);
    }
    
    
    
    /// <summary>
    /// Method <c>SaveNodes</c> serializes and saves all currently active nodes.
    /// <param name="filename">The name of the file to store the serialization within.</param>
    /// <param name="folder">The folder to store it in, if appropriate.</param>
    /// </summary>
    public void Save(string filename, string folder=null)
    {
        if (folder != null) filename = Path.Combine(folder, filename);
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