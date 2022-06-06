using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeTextbox : NodeConnectedObject
{
    [SerializeField] protected GameObject Node;
    protected int NodeId;
    protected TextMeshProUGUI _text;
    protected TMP_InputField _textField;

    /// <summary>
    /// Method <c>Start</c> sets up appropriate components.
    /// </summary>
    public void Start()
    {
        _text = GetComponentInChildren<TextMeshProUGUI>();
        _textField = GetComponent<TMP_InputField>();
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
        if (_textField == null)
        {
            Start();
        }
        NodeId = node_id;
        _textField.text = "Node " + NodeId;
    }



    /// <summary>
    /// Method <c>ValueChanged</c> removes trailing enters and deselects the textbox if there was one.
    /// </summary>
    public void ValueChanged()
    {
        var text_val = _textField.text;
        if (text_val.Length == 0) return;
        if (text_val[text_val.Length - 1] == '\n')
        {
            _textField.text = _textField.text.TrimEnd( '\n' );
            var eventSystem = EventSystem.current;
            if (!eventSystem.alreadySelecting) eventSystem.SetSelectedGameObject (null);
            
        }
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
        _text.transform.position = Camera.main.WorldToScreenPoint(dist + _distFromNode);
        _text.rectTransform.sizeDelta =
            Camera.main.orthographicSize * Node.transform.lossyScale;
    }

    /// <summary>
    /// Method <c>MoveWithNode</c> moves the connector, and any lines, with the node.
    /// <param name="node_id">The ID of the moving node.</param>
    /// <param name="dist">The distance the node has moved.</param>
    /// <param name="is_connecting">Whether the node is currently connecting.</param>
    /// </summary>
    private void MoveWithNode(int node_id, Vector3 dist, bool is_connecting=false)
    {
        if (node_id == NodeId)
        {
            Move(dist);
        }
    }
}