using System;
using UnityEngine;
using DebugAssert = System.Diagnostics.Debug;

/// <summary>
/// Class <c>InteractiveNode</c> provides functionality of a movable interactive node.
/// </summary>
public class InteractiveNode : MonoBehaviour
{
    [SerializeField] protected GameObject sideMenu;
    
    public static event Action<int, Vector3> NodeSelected;
    public static event Action<Vector3> NodeDragged;
    
    private static int NodesCounter;
    private int _nodeId;
    private Vector3 _mouseRelativePos;
    private NodeConnectors _nodeConnectors;

    /// <summary>
    /// Method <c>Start</c> sets the node counter, finds relevant components and disables hidden elements.
    /// </summary>
    public void Start()
    {
        NodesCounter += 1;
        _nodeId = NodesCounter;
        _nodeConnectors = GetComponent<NodeConnectors>();
        _nodeConnectors.SetNodeId(_nodeId);
        sideMenu.GetComponent<NodeSideMenu>().SetPositioning(transform.position);
        sideMenu.GetComponent<NodeSideMenu>().SetNodeId(_nodeId);
        _nodeConnectors.SetupConnectors(transform.position);
        sideMenu.SetActive(false);
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        NodeSelected += NewNodeSelected;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
    }
    
    
    
    /// <summary>
    /// Method <c>OnMouseDown</c> calculates the mouse's relative clicking position.
    /// </summary>
    private void OnMouseDown()
    {
        _mouseRelativePos = gameObject.transform.position - GetMouseCoords();
        sideMenu.SetActive(false);
        NodeSelected?.Invoke(_nodeId, transform.position);
    }

    /// <summary>
    /// Method <c>GetMouseCoords</c> gets the world coordinates of the mouse position.
    /// </summary>
    /// <returns>The world coordinates of the mouse position</returns>
    private Vector3 GetMouseCoords()
    {
        DebugAssert.Assert(Camera.main != null, "Camera.main != null");
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    
    /// <summary>
    /// Method <c>OnMouseDrag</c> moves the node with the mouse as it's dragged.
    /// </summary>
    private void OnMouseDrag()
    {
        transform.position = GetMouseCoords() + _mouseRelativePos;
        if (_nodeConnectors.isConnecting)
        {
            NodeDragged?.Invoke(transform.position);
        }
    }

    /// <summary>
    /// Method <c>OnMouseUp</c> marks the node as selected and reveals the side menu as the node is placed.
    /// </summary>
    private void OnMouseUp()
    {
        sideMenu.SetActive(true);
        NodeSelected?.Invoke(_nodeId, transform.position);
    }

    
    
    /// <summary>
    /// Method <c>NewNodeSelected</c> marks all other nodes as not selected.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// </summary>
    private void NewNodeSelected(int id_selected, Vector3 new_pos)
    {
        if (_nodeId != id_selected)
        {
            sideMenu.SetActive(false);
            _nodeConnectors.DisableConnectors();
        }
    }
}