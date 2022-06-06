using UnityEngine;
using UnityEngine.EventSystems;
using DebugAssert = System.Diagnostics.Debug;

/// <summary>
/// Class <c>InteractiveNode</c> provides functionality of a movable interactive node.
/// </summary>
public class InteractiveNode : BasicNode, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
{
    [SerializeField] protected GameObject sideMenu;
    [SerializeField] public GameObject nameField;
    protected override string NodeType => "Interactive Node";

    /// <summary>
    /// Method <c>Start</c> sets the node counter, finds relevant components and disables hidden elements.
    /// </summary>
    public new void Start()
    {
        NodesCounter += 1;
        nodeId = NodesCounter;
        NodeConnectors = GetComponent<NodeConnectors>();
        NodeConnectors.SetNodeId(nodeId);

        var side_menu_func = sideMenu.GetComponent<NodeSideMenu>();
        side_menu_func.SetPositioning(transform.position);
        side_menu_func.SetNodeId(nodeId);
        var name_textbox = nameField.GetComponent<NodeTextbox>();
        name_textbox.SetNodeId(nodeId);
        name_textbox.Move(transform.position);
        NodeConnectors.SetupConnectors(transform.position);
        sideMenu.SetActive(false);
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        NodeSelected += NewNodeSelected;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        NodeSelected -= NewNodeSelected;
    }
    


    /// <summary>
    /// Method <c>OnPointerDown</c> calculates the mouse's relative clicking position.
    /// </summary>
    public new void OnBeginDrag(PointerEventData event_data)
    {
        sideMenu.SetActive(false);
        base.OnBeginDrag(event_data);
    }

    /// <summary>
    /// Method <c>OnPointerUp</c> marks the node as selected and reveals the side menu as the node is placed.
    /// </summary>
    public new void OnEndDrag(PointerEventData event_data)
    {
        sideMenu.SetActive(true);
        base.OnEndDrag(event_data);
    }

    /// <summary>
    /// Method <c>OnPointerDown</c> marks the node as selected and reveals the side menu.
    /// </summary>
    public new void OnPointerDown(PointerEventData event_data)
    {
        sideMenu.SetActive(true);
        base.OnPointerDown(event_data);
    }



    /// <summary>
    /// Method <c>NewNodeSelected</c> marks all other nodes as not selected.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// <param name="game_obj">The selected node game object.</param>
    /// </summary>
    private void NewNodeSelected(int id_selected, Vector3 new_pos=default, GameObject game_obj=default)
    {
        if (nodeId != id_selected)
        {
            sideMenu.SetActive(false);
            NodeConnectors.DisableConnectors();
        }
    }
}