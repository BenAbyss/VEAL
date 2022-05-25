using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DebugAssert = System.Diagnostics.Debug;

/// <summary>
/// Class <c>NodeConnector</c> provides functionality of nodes' connectors.
/// </summary>
public class NodeConnector  : NodeConnectedObject
{
    [SerializeField] private float[] arrowThickness; // [start_width, end_width, length_percent]
    public static event Action MakingConnection;
    public static event Action<NodeConnector> ConnectionRecipientMade;

    public static bool ConnectionsOccurring;
    public bool isVisible;
    private LineRenderer _lineRenderer;
    private LineRenderer _arrowTipRenderer;
    private NodeConnector _connectionTo;
    private NodeConnector _connectionFrom;
    private NodeConnectors _connectorGroup;
    private SpriteRenderer _renderer;
    private Button _button;
    private bool _connectingNode;
    private Camera _camera;

    /// <summary>
    /// Method <c>Awake</c> sets up all the appropriate components.
    /// </summary>
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _button = GetComponent<Button>();
        _camera = Camera.main;
    }
    
    /// <summary>
    /// Method <c>Update</c> updates the connecting line ot the mouse position, when appropriate.
    /// </summary>
    private void Update()
    {
        if (_connectingNode)
        {
            DrawConnection(GetCoordinates());
        }
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        BasicNode.NodeDragged += MoveWithNode;
        ConnectionRecipientMade += FormConnectionRecipient;
        NodeManager.MidpointCreated += FormMidpointConnection;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        BasicNode.NodeDragged -= MoveWithNode;
        ConnectionRecipientMade -= FormConnectionRecipient;
        NodeManager.MidpointCreated -= FormMidpointConnection;
    }

    
    
    /// <summary>
    /// Method <c>SetConnectorGroup</c> sets the group object of the node.
    /// <param name="connector_group">The node group object.</param>
    /// </summary>
    public void SetConnectorGroup(NodeConnectors connector_group)
    {
        _connectorGroup = connector_group;
    }

    /// <summary>
    /// Method <c>SetConnectionFrom</c> sets the connector that leads to this node.
    /// <param name="connector">The connector that leads to this node.</param>
    /// </summary>
    private void SetConnectionFrom(NodeConnector connector)
    {
        _connectionFrom = connector;
    }

    /// <summary>
    /// Method <c>GetConnectionFromID</c> returns the id of the node with a connection to this connector.
    /// <returns>The connector nodes' ID, or -1 if it doesn't exist.</returns>
    /// </summary>
    public int GetConnectionFromID()
    {
        if (_connectionFrom == null)
        {
            return -1;
        }

        return _connectionFrom.GetNodesId();
    }
    
    /// <summary>
    /// Method <c>GetConnectionFromID</c> returns the id of the node this connector connects to.
    /// <returns>The connectee nodes' ID, or -1 if it doesn't exist.</returns>
    /// </summary>
    public int GetConnectionToID()
    {
        if (_connectionTo == null)
        {
            return -1;
        }
        else
        {
            return _connectionTo.GetNodesId();
        }
    }

    /// <summary>
    /// Method <c>GetNodesId</c> returns the id of this connectors' node.
    /// <returns>This connectors' node.</returns>
    /// </summary>
    private int GetNodesId()
    {
        return _connectorGroup.nodeId;
    }
    
    /// <summary>
    /// Method <c>SetupLineRenderer</c> sets up the Line Renderer with the appropriate settings.
    /// </summary>
    private void SetupLineRenderer()
    {
        gameObject.AddComponent(typeof(LineRenderer));
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = 0.2f;
        _lineRenderer.endWidth = 0.2f;
        _lineRenderer.material = Resources.Load<Material>("Materials/Arrow-Material");

        var arrow_obj = new GameObject();
        arrow_obj.transform.parent = transform;
        arrow_obj.AddComponent(typeof(LineRenderer));
        _arrowTipRenderer = arrow_obj.GetComponent<LineRenderer>();
        _arrowTipRenderer.startWidth = arrowThickness[0];
        _arrowTipRenderer.endWidth = arrowThickness[1];
        _arrowTipRenderer.material = Resources.Load<Material>("Materials/Arrow-Material");
    }
    


    /// <summary>
    /// Method <c>ChangeVisibility</c> changes the visibility and interactability of the node.
    /// <param name="is_visible">Whether the connector is visible or not.</param>
    /// </summary>
    public void ChangeVisibility(bool is_visible)
    {
        _renderer.enabled = is_visible;
        _button.interactable = is_visible;
        isVisible = is_visible;
    }

    /// <summary>
    /// Method <c>ApplyConnectedSprite</c> changes the sprite used to a green one.
    /// </summary>
    private void ApplyConnectedSprite()
    {
        _renderer.sprite = Resources.Load<Sprite>("Sprites/NodeConnectors/ConnectedNodeConnector");
    }
    
    /// <summary>
    /// Method <c>ApplyDisconnectedSprite</c> changes the sprite used to a red one.
    /// </summary>
    private void ApplyDisconnectedSprite()
    {
        _renderer.sprite = Resources.Load<Sprite>("Sprites/NodeConnectors/DisconnectedNodeConnector");
    }
    
    /// <summary>
    /// Method <c>Clicked</c> makes other nodes' connectors visible, providing the node isn't connected already.
    /// </summary>
    public void Clicked()
    {
        if (_lineRenderer == null)
        {
            SetupLineRenderer();
        }

        if (_connectionTo != null)
        {
            _connectionTo.ApplyDisconnectedSprite();
            ApplyDisconnectedSprite();
            _connectionTo = null;
        }
        
        MakingConnection?.Invoke();
        if (_connectorGroup.isConnecting)
        {
            ConnectionsOccurring = true;
            if (_connectionTo == null)
            {
                _connectingNode = true;
            }
        }
        else
        {
            ConnectionsOccurring = false;
            ConnectionRecipientMade?.Invoke(this);
        }

    }
    
    /// <summary>
    /// Method <c>GetCoordinates</c> gets the coordinates of the mouse, for line mapping.
    /// </summary>
    /// This is done more complexly than the InteractiveNode alternative, as it requires a consistent
    /// z-axis with constant thickness.
    private Vector3 GetCoordinates()
    {
        var ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        var plane = new Plane(Vector3.forward, new Vector3(0, 0, -2));
        plane.Raycast(ray, out var distance);
        return ray.GetPoint(distance);
    }
    
    /// <summary>
    /// Method <c>MoveWithNode</c> moves the connector, and any lines, with the node.
    /// <param name="node_id">The ID of the moving node.</param>
    /// <param name="dist">The distance the node has moved.</param>
    /// <param name="is_connecting">Whether the node is currently connecting.</param>
    /// </summary>
    private void MoveWithNode(int node_id, Vector3 dist, bool is_connecting=false)
    {
        if (node_id == _connectorGroup.nodeId)
        {
            Move(node_id, dist);
            if (_connectionTo != null)
            {
                DrawConnection();
            }
            if (_connectionFrom != null)
            {
                _connectionFrom.DrawConnection();
            }
        }
    }

    /// <summary>
    /// Method <c>DrawConnection</c> draws the connection between the two nodes.
    /// </summary>
    private void DrawConnection(Vector3 coords=default)
    {
        var start = transform.position - new Vector3(0.05f, 0.05f, 0);
        start.z = _connectorGroup.GetNodeDepth() + 0.1f;
        var end = coords != default ? coords : _connectionTo.transform.position;
        end.z = _connectorGroup.GetNodeDepth() + 0.1f;
        
        var arrow_length = (Vector3.Distance(start, end) < 1.75f) ? 0.5f : (100 - arrowThickness[2]) / 100;
        
        _lineRenderer.numCapVertices = 5;
        _lineRenderer.SetPositions(new[]{start, Vector3.Lerp(start, end, arrow_length)});
        _arrowTipRenderer.SetPositions(new[]{Vector3.Lerp(start, end, arrow_length), end});
    }

    /// <summary>
    /// Method <c>FormConnectionRecipient</c> forms a connection to the selected connector.
    /// <param name="recipient">The receiving connector of the connection.</param>
    /// </summary>
    private void FormConnectionRecipient(NodeConnector recipient)
    {
        if (_connectingNode && !_connectorGroup.CheckForConnectedNode(recipient.GetNodesId()))
        {
            _connectionTo = recipient;
            _connectionTo.SetConnectionFrom(this);
            DrawConnection();
            ApplyConnectedSprite();
            _connectionTo.ApplyConnectedSprite();
        }
        else if (_connectingNode)
        {
            _lineRenderer.positionCount = 0;
            _arrowTipRenderer.positionCount = 0;
        }
        _connectingNode = false;
    }

    private void FormMidpointConnection(Midpoint midpt)
    {
        if (!_connectingNode) return;
        var angle = Mathf.Atan2(transform.position.y - midpt.transform.position.y,
            transform.position.x - midpt.transform.position.x) * Mathf.Rad2Deg - 90;
        var pos = "";
        
        switch (angle)
        {
            case var i when -45 < i && i <= 45:
                pos = "Top";
                break;
            case var i when -135 < i && i <= -45:
                pos = "Right";
                break;
            //case var i when 135 < i || i <= -135:
                //pos = "Btm";
                //break;
            case var i  when 45 < i && i <= 135:
                pos = "Left";
                break;
            default:
                pos = "Btm";
                break;
        }

        FormConnectionRecipient(midpt.GetConnector(pos + "Conn"));
    }
}