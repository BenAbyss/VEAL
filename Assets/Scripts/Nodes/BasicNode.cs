using System;
using UnityEngine;
using UnityEngine.EventSystems;
using DebugAssert = System.Diagnostics.Debug;

public class BasicNode: MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    public static event Action<int, Vector3, GameObject> NodeSelected;
    public static event Action<int, Vector3, bool> NodeDragged;
    
    public static int NodesCounter;
    protected virtual string NodeType => "Basic Node";
    protected virtual int OutputLimit => 20;
    public int nodeId;
    public new string name;
    private Vector3 _mouseRelativePos;
    protected NodeConnectors NodeConnectors;

    /// <summary>
    /// Method <c>Awake</c> sets the node counter & finds relevant components.
    /// </summary>
    public void Awake()
    {
        NodesCounter += 1;
        nodeId = NodesCounter;
        NodeConnectors = GetComponent<NodeConnectors>();
        NodeConnectors.SetNodeId(nodeId);
        NodeConnectors.SetOutputLimit(OutputLimit);
    }
    
    /// <summary>
    /// Method <c>Start</c> further sets up the connectors.
    /// </summary>
    public void Start()
    {
        NodeConnectors.SetupConnectors(transform.position);
        NodeConnectors.SetNodeType(NodeType);
        NodeConnectors.SetOutputLimit(OutputLimit);
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
    /// Method <c>GetNodeConnectors</c> gets the connectors group for the node.
    /// <returns>The node connector group.</returns>
    /// </summary>
    public NodeConnectors GetNodeConnectors()
    {
        return NodeConnectors;
    }
    
    /// <summary>
    /// Method <c>GetNodeType</c> gets the string type of the node.
    /// <returns>The node type.</returns>
    /// </summary>
    public string GetNodeType()
    {
        return NodeType;
    }

    /// <summary>
    /// Method <c>GetOutputLimit</c> gets the output limit of the node.
    /// <returns>The output limit.</returns>
    /// </summary>
    public int GetOutputLimit()
    {
        return OutputLimit;
    }
    
    /// <summary>
    /// Method <c>SetNodeName</c> sets the node's name.
    /// <param name="new_name">The new name for the node.</param>
    /// </summary>
    public virtual void SetNodeName(string new_name)
    {
        name = new_name;
    }

    /// <summary>
    /// Method <c>SetNodeId</c> sets the node's id.
    /// This should ONLY be used for deserialization.
    /// <param name="new_id">The new id for the node.</param>
    /// </summary>
    public virtual void SetNodeId(int new_id)
    {
        nodeId = new_id;
        NodeConnectors.SetNodeId(nodeId);
    }
    
    
    
    /// <summary>
    /// Method <c>OnPointerDown</c> calculates the mouse's relative clicking position.
    /// </summary>
    public void OnBeginDrag(PointerEventData event_data)
    {
        _mouseRelativePos = gameObject.transform.position - InputManager.GetMouseCoords();
        NodeSelected?.Invoke(nodeId, transform.position, gameObject);
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
    /// Method <c>OnEndDrag</c> marks the node as selected as the drag is released.
    /// </summary>
    public void OnEndDrag(PointerEventData event_data)
    {
        NodeSelected?.Invoke(nodeId, transform.position, gameObject);
    }

    /// <summary>
    /// Method <c>OnPointerDown</c> marks the node as selected on a left click.
    /// </summary>
    public void OnPointerDown(PointerEventData event_data)
    {
        if (event_data.button == 0)
        {
            NodeSelected?.Invoke(nodeId, transform.position, gameObject);
        }
    }



    /// <summary>
    /// Method <c>ChangeColour</c> changes the colour of the nodes internals.
    /// <param name="colour">The colour value to set it to.</param>
    /// </summary>
    public void ChangeColour(Color colour)
    {
        GetComponent<SpriteRenderer>().color = colour;
    }

    /// <summary>
    /// Method <c>NewNodeSelected</c> marks all other nodes as not selected.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// <param name="game_obj">The selected node game object.</param>
    /// </summary>
    protected void NewNodeSelected(int id_selected, Vector3 new_pos=default, GameObject game_obj=default)
    {
        if (nodeId != id_selected)
        {
            NodeConnectors.DisableConnectors();
        }
    }
}