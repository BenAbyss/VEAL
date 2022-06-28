using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class NodeSave : SaveSystem
{
    public static event Action NewSavedNode;
    private NodeCreatorMenuManager _creatorManager;
    protected override string Subdirectory => "Nodes";

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        NodeManager.SaveNode += SaveNode;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        NodeManager.SaveNode -= SaveNode;
    }



    public void SetCreatorManager(NodeCreatorMenuManager manager)
    {
        _creatorManager = manager;
    }
    
    /// <summary>
    /// Method <c>LoadNode</c> loads all saved nodes from serialization.
    /// <returns>All saved nodes.</returns>
    /// </summary>
    public List<GameObject> BuildAllSavedNodePanels()
    {
        var panels = new List<GameObject>();
        var files = Directory.GetFiles(SubPath());
        var nodes_made = 0;

        foreach (var file in files)
        {
            using (var file_reader = new StreamReader(file))
            {
                var node = LoadNode(file_reader.ReadLine(), true);
                panels.Add(_creatorManager.BuildSavedNodePanel(node));
                nodes_made++;
            }
        }

        // undo the unnecessary incrementation of the nodes counter
        BasicNode.NodesCounter -= nodes_made;
        return panels;
    }

    /// <summary>
    /// Method <c>SaveNodes</c> serializes and saves a given node.
    /// <param name="node">The node to save.</param>
    /// <param name="filename">The name of the file to store the serialization within.</param>
    /// </summary>
    private void SaveNode(GameObject node, string filename)
    {
        using (var output = new StreamWriter(NewFilePath(filename)))
        {
            output.WriteLine(SerializeNode(node));
        }
        NewSavedNode?.Invoke();
    }



    /// <summary>
    /// Method <c>SaveNodes</c> serializes a given node.
    /// <param name="node">The node to serialize.</param>
    /// <returns>The serialized string of the node.</returns>
    /// </summary>
    protected static string SerializeNode(GameObject node)
    {
        return JsonUtility.ToJson(new SerializedNode(node.gameObject));
    }

    /// <summary>
    /// Method <c>LoadNode</c> loads and instantiates a given node from serialization.
    /// <param name="json_str">The JSON string to deserialize from.</param>
    /// </summary>
    protected GameObject LoadNode(string json_str, bool as_prefab=false)
    {
        var node_json = JsonUtility.FromJson<SerializedNode>(json_str);
        var prefab_name = "Prefabs/Nodes/" +
                          $"{Regex.Replace(node_json.nodeType, @"\s+", "")}";
        var node = Instantiate(Resources.Load(prefab_name), 
            GameObject.Find("MainCanvas").transform) as GameObject;
        var node_obj = node.GetComponentInChildren<BasicNode>();
            
        node.transform.position = node_json.position;
        node_obj.SetNodeId(node_json.nodeId);
        node_obj.SetNodeName(node_json.nodeName);
        var sprite_renderer = node_json.nodeType != "Midpoint" ? 
            node.transform.Find("ConnectableNode").Find("MainNode").GetComponent<SpriteRenderer>() : 
            node.transform.Find("Midpoint").GetComponent<SpriteRenderer>();
        sprite_renderer.color = node_json.colour;


        if (!as_prefab) return node;
        
        // delete the node and create a prefab in it's place
        var node_name = ValidName("Node");
        if (!string.IsNullOrEmpty(node_obj.name))
        {
            node_name = node_obj.name;
        }
        var prefab = PrefabUtility.SaveAsPrefabAsset(node, "Assets/Temporary/"+node_name+".prefab");
        Destroy(node);
        return prefab;
    }
}