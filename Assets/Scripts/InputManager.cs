using DebugAssert = System.Diagnostics.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private NodeManager _nodeManager;
    
    /// <summary>
    /// Method <c>Start</c> finds the node manager component.
    /// </summary>
    public void Start()
    {
        _nodeManager = GetComponent<NodeManager>();
    }

    /// <summary>
    /// Method <c>OnRightClick</c> creates a midpoint upon right clicking while a connection arrow is being made.
    /// </summary>
    public void OnRightClick()
    {
        if (NodeConnector.ConnectionsOccurring)
        {
            StartCoroutine(_nodeManager.CreateMidpoint());
        }
    }
    
    /// <summary>
    /// Method <c>GetMouseCoords</c> gets the world coordinates of the mouse position.
    /// </summary>
    /// <returns>The world coordinates of the mouse position</returns>
    public static Vector3 GetMouseCoords()
    {
        DebugAssert.Assert(Camera.main != null, "Camera.main != null");
        return Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}