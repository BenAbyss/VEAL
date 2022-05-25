using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DebugAssert = System.Diagnostics.Debug;

public class BasicNode: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public static event Action<int, Vector3> NodeSelected;
    public static event Action<int, Vector3, bool> NodeDragged;
    
    protected static int NodesCounter;
    public int nodeId;
    private Vector3 _mouseRelativePos;
    protected NodeConnectors NodeConnectors;

    /// <summary>
    /// Method <c>Start</c> sets the node counter, finds relevant components and disables hidden elements.
    /// </summary>
    public void Start()
    {
        NodesCounter += 1;
        nodeId = NodesCounter;
        NodeConnectors = GetComponent<NodeConnectors>();
        NodeConnectors.SetNodeId(nodeId);
        NodeConnectors.SetupConnectors(transform.position);
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        NodeSelected += NewNodeSelected;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
    }
    
    
    
    /// <summary>
    /// Method <c>OnPointerDown</c> calculates the mouse's relative clicking position.
    /// </summary>
    public void OnBeginDrag(PointerEventData event_data)
    {
        _mouseRelativePos = gameObject.transform.position - InputManager.GetMouseCoords();
        NodeSelected?.Invoke(nodeId, transform.position);
    }

    /// <summary>
    /// Method <c>OnDrag</c> moves the node with the mouse as it's dragged.
    /// </summary>
    public void OnDrag(PointerEventData event_data)
    {
        transform.position = InputManager.GetMouseCoords() + _mouseRelativePos;
        NodeDragged?.Invoke(nodeId, transform.position, NodeConnectors.isConnecting);
    }

    /// <summary>
    /// Method <c>OnPointerUp</c> marks the node as selected and reveals the side menu as the node is placed.
    /// </summary>
    public void OnEndDrag(PointerEventData event_data)
    {
        NodeSelected?.Invoke(nodeId, transform.position);
    }

    public void OnPointerDown(PointerEventData event_data)
    {
        if (event_data.button == 0)
        {
            NodeSelected?.Invoke(nodeId, transform.position);
        }
    }



    /// <summary>
    /// Method <c>NewNodeSelected</c> marks all other nodes as not selected.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// </summary>
    private void NewNodeSelected(int id_selected, Vector3 new_pos=default)
    {
        if (nodeId != id_selected)
        {
            NodeConnectors.DisableConnectors();
        }
    }
}