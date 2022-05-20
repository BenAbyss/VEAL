using System;
using UnityEngine;

/// <summary>
/// Class <c>NodeSideMenu</c> controls interactivity of the node's side menu.
/// </summary>
public class NodeSideMenu : MonoBehaviour
{
    [SerializeField] protected GameObject _renameBtn;
    [SerializeField] protected GameObject _connectBtn;
    [SerializeField] protected GameObject _editBtn;

    private Vector3 _distFromNode;

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        InteractiveNode.NodeSelected += MoveBtns;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        InteractiveNode.NodeSelected -= MoveBtns;
    }

    
    
    
    /// <summary>
    /// Method <c>SetPositioning</c> sets the positioning of the menu relative to the node.
    /// <param name="node_pos">The new position of the connected node.</param>
    /// </summary>
    public void SetPositioning(Vector3 node_pos)
    {
        _distFromNode = transform.position - node_pos;
    }

    /// <summary>
    /// Method <c>MoveBtns</c> moves the buttons to their new position.
    /// <param name="node_id">The id of the selected node.</param>
    /// <param name="dist">The new position of the selected node.</param>
    /// </summary>
    public void MoveBtns(int node_id, Vector3 dist)
    {
        transform.position = dist + _distFromNode;
    }
    
    /// <summary>
    /// Method <c>Rename</c> lets the user rename the node.
    /// </summary>
    public void Rename()
    {
        Debug.Log("Rename");
    }
    
    /// <summary>
    /// Method <c>Connect</c> enables the node connectors.
    /// </summary>
    public void Connect()
    {
        Debug.Log("Connect");
    }
    
    /// <summary>
    /// Method <c>Edit</c> opens up the node edit menu.
    /// </summary>
    public void Edit()
    {
        Debug.Log("Edit");
    }
}