using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
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
    [SerializeField] private float lineThickness;
    [SerializeField] private string ioType;
    public static event Action MakingConnection;
    public static event Action<NodeConnector> ConnectionRecipientMade;
    public static event Action<NodeConnectors> ExcessOutputMade;

    public static bool ConnectionsOccurring;
    public bool isVisible;
    public NodeConnectors connectorGroup;
    private LineRenderer _lineRenderer;
    private LineRenderer _arrowTipRenderer;
    private NodeConnector _connectionTo;
    private NodeConnector _connectionFrom;
    private SpriteRenderer _renderer;
    private Button _button;
    private bool _connectingNode;

    /// <summary>
    /// Method <c>Awake</c> sets up all the appropriate components.
    /// </summary>
    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _button = GetComponent<Button>();
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
    private new void OnEnable()
    {
        BasicNode.NodeDragged += MoveWithNode;
        ConnectionRecipientMade += FormConnectionRecipient;
        NodeManager.MidpointCreated += FormMidpointConnection;
        ExcessOutputMade += PulseLines;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private new void OnDisable()
    {
        BasicNode.NodeDragged -= MoveWithNode;
        ConnectionRecipientMade -= FormConnectionRecipient;
        NodeManager.MidpointCreated -= FormMidpointConnection;
        ExcessOutputMade -= PulseLines;
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
    /// Method <c>SetConnectionTo</c> sets the connector that this node leads to.
    /// <param name="connector">The connector that this node leads to.</param>
    /// </summary>
    private void SetConnectionTo(NodeConnector connector)
    {
        _connectionTo = connector;
    }
    
    /// <summary>
    /// Method <c>ClearConnections</c> clears any connections from the connector.
    /// </summary>
    public void ClearConnections()
    {
        if (_connectionFrom != null)
        {
            _connectionFrom.SetConnectionTo(null);
            _connectionFrom.RemoveDrawings();
            _connectionFrom.ApplyDisconnectedSprite();
            _connectionFrom = null;
        }
        
        if (_connectionTo != null)
        {
            _connectionTo.SetConnectionFrom(null);
            _connectionTo.ApplyDisconnectedSprite();
            _connectionTo = null;
            RemoveDrawings();
        }
    }

    /// <summary>
    /// Method <c>SetConnectingNode</c> sets the connecting node boolean.
    /// <param name="connecting_node">The new boolean value.</param>
    /// </summary>
    public void SetConnectingNode(bool connecting_node)
    {
        _connectingNode = connecting_node;
    }
    
    /// <summary>
    /// Method <c>GetConnectingNode</c> gets the connecting node boolean.
    /// <returns>Whether the node is currently connecting.</returns>
    /// </summary>
    public bool GetConnectingNode()
    {
        return _connectingNode;
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

        return _connectionTo.GetNodesId();
    }
    
    /// <summary>
    /// Method <c>GetConnectionTo</c> returns node connector this connector connects to.
    /// <returns>The connector object.</returns>
    /// </summary>
    public NodeConnector GetConnectionTo()
    {
        return _connectionTo;
    }
    
    /// <summary>
    /// Method <c>GetConnectorToName</c> gets the name of the connector this connector outputs to.
    /// <returns>The name of the connector.</returns>
    /// </summary>
    public string GetConnectorToName()
    {
        return _connectionTo.connectorGroup.connectors.First(connector => 
            connector.Value.GetComponent<NodeConnector>() == _connectionTo).Key;
    }
    
    /// <summary>
    /// Method <c>GetConnectionFrom</c> returns node connector that connects to this.
    /// <returns>The connector object.</returns>
    /// </summary>
    public NodeConnector GetConnectionFrom()
    {
        return _connectionFrom;
    }
    
    /// <summary>
    /// Method <c>HasOutput</c> states if the node has an outgoing connection.
    /// <returns>A boolean stating whether it has a connection.</returns>
    /// </summary>
    public bool HasOutput()
    {
        return _connectionTo != null;
    }
    
    /// <summary>
    /// Method <c>HasInput</c> states if the node has an incoming connection.
    /// <returns>A boolean stating whether it has a connection.</returns>
    /// </summary>
    public bool HasInput()
    {
        return _connectionFrom != null;
    }

    /// <summary>
    /// Method <c>GetNodesId</c> returns the id of this connectors' node.
    /// <returns>This connectors' node.</returns>
    /// </summary>
    private int GetNodesId()
    {
        return connectorGroup.nodeId;
    }
    
    /// <summary>
    /// Method <c>SetupLineRenderer</c> sets up the Line Renderer with the appropriate settings.
    /// </summary>
    private void SetupLineRenderer()
    {
        gameObject.AddComponent(typeof(LineRenderer));
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = lineThickness;
        _lineRenderer.endWidth = lineThickness;
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
        if (_renderer == null)
        {
            Awake();
        }
        _renderer.enabled = is_visible;
        _button.interactable = is_visible;
        isVisible = is_visible;
    }

    /// <summary>
    /// Method <c>RemoveDrawings</c> removes drawn lines from the connector.
    /// </summary>
    public void RemoveDrawings()
    {
        if (_lineRenderer != null)
        {
            _lineRenderer.positionCount = 0;
            _arrowTipRenderer.positionCount = 0;
        }
    }
    
    /// <summary>
    /// Method <c>ApplyConnectedSprite</c> changes the sprite used to a green one.
    /// </summary>
    private void ApplyConnectedSprite()
    {
        _renderer.sprite = Resources.Load<Sprite>("Sprites/Node Connectors/ConnectedNodeConnector");
    }
    
    /// <summary>
    /// Method <c>ApplyDisconnectedSprite</c> changes the sprite used to a red one.
    /// </summary>
    private void ApplyDisconnectedSprite()
    {
        _renderer.sprite = Resources.Load<Sprite>("Sprites/Node Connectors/DisconnectedNodeConnector");
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
            connectorGroup.DecrementOutputCount();
        }
        
        MakingConnection?.Invoke();
        if (connectorGroup.isConnecting && ioType != "Input")
        {
            ConnectionsOccurring = true;
            if (_connectionTo == null)
            {
                _connectingNode = true;
            }
        }
        else if (ioType != "Output")
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
        var ray = Camera.ScreenPointToRay(Mouse.current.position.ReadValue());
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
        if (node_id == connectorGroup.nodeId)
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
        var start = transform.position - new Vector3(arrowThickness[1], arrowThickness[1], 0);
        start.z = connectorGroup.GetNodeDepth() + 0.1f;
        var end = coords != default ? coords : _connectionTo.transform.position;
        end.z = connectorGroup.GetNodeDepth() + 0.1f;
        
        // if the distance is less than 6 connectors, have half the line as the arrow tip
        var arrow_length = (Vector3.Distance(start, end) < _renderer.size.x*6) 
            ? 0.5f : (100 - arrowThickness[2]) / 100;
        _lineRenderer.positionCount = 2;
        _arrowTipRenderer.positionCount = 2;
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
        if (_connectingNode && !connectorGroup.CheckForConnectedNode(recipient.GetNodesId()) && 
            !NodeManager.MidpointPathHitsNode(recipient.GetNodesId(), this) 
            && !connectorGroup.OutputLimitReached())
        {
            connectorGroup.IncrementOutputCount();
            _connectionTo = recipient;
            _connectionTo.SetConnectionFrom(this);
            DrawConnection();
            ApplyConnectedSprite();
            _connectionTo.ApplyConnectedSprite();
        }
        else if (_connectingNode)
        {
            RemoveDrawings();
            if (connectorGroup.OutputLimitReached())
            {
                ExcessOutputMade?.Invoke(connectorGroup);
            }
        }

        _connectingNode = false;
    }
    
    /// <summary>
    /// Method <c>FormLoadedConnection</c> forms a connection to the selected connector, under the
    /// assumption it's a valid connection.
    /// <param name="recipient">The receiving connector of the connection.</param>
    /// <param name="connector_group">The nodes connector group.</param>
    /// <param name="recip_conn_group">The recipient nodes connector group.</param>
    /// </summary>
    public void FormLoadedConnection(NodeConnector recipient, NodeConnectors connector_group, 
        NodeConnectors recip_conn_group)
    {
        connectorGroup = connector_group;
        SetupLineRenderer();
        
        connectorGroup.IncrementOutputCount();
        if (recip_conn_group.nodeType == "Midpoint")
        {
            ((Midpoint)recip_conn_group.GetConnectedNodeFunc()).AddParent(connectorGroup.GetConnectedNodeFunc());
        }
        _connectionTo = recipient;
        _connectionTo.SetConnectionFrom(this);
        DrawConnection();
        ApplyConnectedSprite();
        _connectionTo.ApplyConnectedSprite();
    }

    /// <summary>
    /// Method <c>FormMidpointConnection</c> forms a connection to the new midpoint.
    /// <param name="midpt">The new midpoint to connect to.</param>
    /// </summary>
    private void FormMidpointConnection(Midpoint midpt)
    {
        if (!_connectingNode) return;
        
        // add the node to the midpoints' parents, and cancel if the midpoints' outputs would
        // push this node over it's limit
        midpt.AddParent(connectorGroup.GetConnectedNodeFunc());
        if (connectorGroup.OutputLimitExceeded())
        {
            midpt.RemoveParent(connectorGroup.GetConnectedNodeFunc());
            return;
        }
        
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
            case var i  when 45 < i && i <= 135:
                pos = "Left";
                break;
            default:
                pos = "Btm";
                break;
        }

        FormConnectionRecipient(midpt.GetConnector(pos + "Conn"));
    }

    private void PulseLines(NodeConnectors conn_group)
    {
        if (_arrowTipRenderer != null && (conn_group == connectorGroup || 
                                          (connectorGroup.nodeType == "Midpoint" && 
                                           NodeManager.MidpointPathHitsNode(conn_group.nodeId, this))))
        {
            DOTween.Sequence()
                .Append(_lineRenderer.material.DOColor(new Color(0.82f, 0.15f, 0.15f), 1f))
                .Join(_arrowTipRenderer.material.DOColor(new Color(0.82f, 0.15f, 0.15f), 1f))
                .Append(_lineRenderer.material.DOColor(Color.black, 1f))
                .Join(_arrowTipRenderer.material.DOColor(Color.black, 1f));
        }
    }
}