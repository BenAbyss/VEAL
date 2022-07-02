using UnityEngine;

public class NodeConnectedObject : MonoBehaviour
{
    protected Vector3 _distFromNode;
    protected Camera Camera;

    /// <summary>
    /// Method <c>Start</c> sets up needed components.
    /// </summary>
    public void Start()
    {
        Camera = Camera.main;
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        InteractiveNode.NodeSelected += Move;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        InteractiveNode.NodeSelected -= Move;
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
    /// Method <c>Move</c> moves the buttons to their new position.
    /// <param name="node_id">The id of the selected node.</param>
    /// <param name="dist">The new position of the selected node.</param>
    /// <param name="game_obj">The selected node game object.</param>
    /// </summary>
    protected void Move(int node_id, Vector3 dist, GameObject game_obj=default)
    {
        transform.position = dist + _distFromNode;
    }
    
    /// <summary>
    /// Method <c>Move</c> moves the buttons to their new position.
    /// <param name="dist">The new position of the selected node.</param>
    /// </summary>
    public void Move(Vector3 dist)
    {
        transform.position = dist + _distFromNode;
    }
}