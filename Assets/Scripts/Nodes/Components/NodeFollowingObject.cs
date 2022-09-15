using UnityEngine;

public class NodeFollowingObject : NodeConnectedObject
{
    [SerializeField] protected GameObject Node;
    protected int NodeId;
    private RectTransform objRect;

    public new void Start()
    {
        base.Start();
        objRect = gameObject.GetComponent<RectTransform>();
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        BasicNode.NodeDragged += MoveWithNode;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        BasicNode.NodeDragged -= MoveWithNode;
    }
    

    
    /// <summary>
    /// Method <c>SetNodeId</c> sets the connected nodes' ID.
    /// <param name="node_id">The id of the selected node.</param>
    /// </summary>
    public void SetNodeId(int node_id)
    {
        if (objRect == null)
        {
            Start();
        }
        NodeId = node_id;
    }
    
    /// <summary>
    /// Method <c>EditEnded</c> marks the inputs as no longer typing.
    /// </summary>
    public void EditEnded()
    {
        InputManager.isTyping = false;
    }
    
    /// <summary>
    /// Method <c>Move</c> moves the buttons to their new position.
    /// <param name="dist">The new position of the selected node.</param>
    /// </summary>
    public new void Move(Vector3 dist)
    {
        gameObject.transform.position = Camera.main.WorldToScreenPoint(dist + _distFromNode);
        objRect.sizeDelta =
            Camera.main.orthographicSize * Node.transform.lossyScale;
    }

    /// <summary>
    /// Method <c>MoveWithNode</c> moves the connector, and any lines, with the node.
    /// <param name="node_id">The ID of the moving node.</param>
    /// <param name="dist">The distance the node has moved.</param>
    /// <param name="is_connecting">Whether the node is currently connecting.</param>
    /// </summary>
    protected void MoveWithNode(int node_id, Vector3 dist, bool is_connecting=false)
    {
        if (node_id == NodeId)
        {
            Move(dist);
        }
    }
}
