using System;
using DebugAssert = System.Diagnostics.Debug;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private NodeCreatorMenuManager creatorMenuManager;
    private NodeManager _nodeManager;

    public static bool isTyping;
    public static event Action CancelAction;
    
    /// <summary>
    /// Method <c>Start</c> finds the node manager component.
    /// </summary>
    public void Start()
    {
        isTyping = false;
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
    /// Method <c>OnCancel</c> invokes a cancelling action upon a cancel button being pressed.
    /// </summary>
    public void OnCancel()
    {
        CancelAction?.Invoke();
    }

    /// <summary>
    /// Method <c>OnDelete</c> deletes the currently selected node.
    /// </summary>
    public void OnDelete()
    {
        _nodeManager.DestroySelectedNode();
    }

    /// <summary>
    /// Method <c>OnOpenCreator</c> toggles the creator menu.
    /// </summary>
    public void OnOpenCreator()
    {
        if (!isTyping)
        {
            creatorMenuManager.ToggleActive();
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