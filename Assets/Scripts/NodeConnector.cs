using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using DebugAssert = System.Diagnostics.Debug;

/// <summary>
/// Class <c>NodeConnector</c> provides functionality of nodes' connectors.
/// </summary>
public class NodeConnector  : NodeConnectedObject
{
    public static event Action MakingConnection;
    public static event Action<NodeConnector> ConnectionRecipientMade;

    public bool isVisible;
    private LineRenderer _lineRenderer;
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
        InteractiveNode.NodeDragged += MoveWithNode;
        ConnectionRecipientMade += FormConnectionRecipient;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        InteractiveNode.NodeDragged -= MoveWithNode;
        ConnectionRecipientMade -= FormConnectionRecipient;
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
        else
        {
            return _connectionFrom.GetNodesId();
        }
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
    /// Method <c>Clicked</c> makes other nodes' connectors visible, providing the node isn't connected already.
    /// </summary>
    public void Clicked()
    {
        if (_lineRenderer == null)
        {
            SetupLineRenderer();
        }
        
        MakingConnection?.Invoke();
        if (_connectorGroup.isConnecting)
        {
            if (_connectionTo == null)
            {
                _connectingNode = true;
            }

            else
            {
                // React to node already being connected
            }
        }
        else
        {
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
        var ray = _camera.ScreenPointToRay(Input.mousePosition);
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
        if (coords != default)
        {
            _lineRenderer.SetPositions(new[]
                {transform.position, coords});
        }
        else
        {
            _lineRenderer.SetPositions(new[]
                {transform.position, _connectionTo.transform.position});   
        }
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
            _connectingNode = false;
            DrawConnection();
            ApplyConnectedSprite();
            _connectionTo.ApplyConnectedSprite();
        }
    }
}