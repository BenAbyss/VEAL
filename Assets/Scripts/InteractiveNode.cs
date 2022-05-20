using System;
using System.Collections.Generic;
using UnityEngine;
using DebugAssert = System.Diagnostics.Debug;

/// <summary>
/// Class <c>InteractiveNode</c> provides functionality of a movable interactive node.
/// </summary>
public class InteractiveNode : MonoBehaviour
{
    public static event Action<int, Vector3> NodeSelected;
    public static int NodesCounter = 0;
    
    [SerializeField] protected GameObject sideMenu;
    [SerializeField] protected Dictionary<string, GameObject> connectors;
    
    private int _nodeId;
    private bool _selected;
    private Vector3 _mouseRelativePos;

    /// <summary>
    /// Method <c>Start</c> sets the node counter, finds relevant components and disables hidden elements.
    /// </summary>
    public void Start()
    {
        _selected = false;
        NodesCounter += 1;
        _nodeId = NodesCounter;
        sideMenu.GetComponent<NodeSideMenu>().SetPositioning(transform.position);
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
        _selected = true;
        sideMenu.SetActive(false);
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
            _selected = false;
            sideMenu.SetActive(false);
        }
    }
}