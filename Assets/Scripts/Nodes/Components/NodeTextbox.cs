using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NodeTextbox : NodeConnectedObject
{
    [SerializeField] protected GameObject Node;
    private InteractiveNode _nodeFunc;
    protected int NodeId;
    protected TextMeshProUGUI _text;
    protected TMP_InputField _textField;

    /// <summary>
    /// Method <c>Awake</c> sets up appropriate components.
    /// </summary>
    public void Awake()
    {
        _nodeFunc = Node.GetComponent<InteractiveNode>();
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
            Awake();
        }
        NodeId = node_id;

        // sets the text file to the node id, or 50% if it's a probability node
        if (Node.GetComponent<InteractiveNode>().GetNodeType() == "Probability Node")
        {
            _textField.text = "50%";
        }
        else
        {
            _textField.text = "Node " + NodeId;
        }
        _nodeFunc.SetNodeName(_textField.text);
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
        
        // if it's a probability node, remove any non-numbers and add a %
        if (Node.GetComponent<InteractiveNode>().GetNodeType() == "Probability Node")
        {
            _textField.text = Regex.Replace(_textField.text, "[^0-9]", "") + "%";
        }
        _nodeFunc.SetNodeName(_textField.text);
    }

    /// <summary>
    /// Method <c>UpdateValueToName</c> updates the text value to the new name.
    /// <param name="new_name">The new name of the node.</param>
    /// </summary>
    public void UpdateValueToName(string new_name)
    {
        _textField.text = new_name;
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