using System;
using TMPro;
using UnityEngine;

/// <summary>
/// Class <c>NodeSideMenu</c> controls interactivity of the node's side menu.
/// </summary>
public class NodeSideMenu : NodeConnectedObject
{
    [SerializeField] private GameObject node;
    private int _nodeId;
    
    public static event Action<int> Connecting;
    public static event Action<InteractiveNode> Editing;
    
    /// <summary>
    /// Method <c>Rename</c> lets the user rename the node.
    /// </summary>
    public void Rename()
    {
        InputManager.isTyping = true;
        node.GetComponent<InteractiveNode>().nameField.GetComponent<TMP_InputField>().Select();
    }
    
    /// <summary>
    /// Method <c>Connect</c> enables the node connectors.
    /// </summary>
    public void Connect()
    {
        Connecting?.Invoke(_nodeId);
    }
    
    /// <summary>
    /// Method <c>Edit</c> opens up the node edit menu.
    /// </summary>
    public void Edit()
    {
        Editing?.Invoke(node.GetComponentInChildren<InteractiveNode>());
    }

    /// <summary>
    /// Method <c>SetNodeId</c> sets the ID of the menu's relevant node.
    /// <param name="node_id">The ID of the relevant node.</param>
    /// </summary>
    public void SetNodeId(int node_id)
    {
        _nodeId = node_id;
    }
}