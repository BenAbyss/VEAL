using System;
using UnityEngine;

/// <summary>
/// Class <c>NodeConnector</c> provides functionality of nodes' connectors.
/// </summary>
public class NodeConnector  : NodeConnectedObject
{
    public static event Action MakingConnection;
    private GameObject _connection;
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    private void OnEnable()
    {
        InteractiveNode.NodeDragged += Move;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    private void OnDisable()
    {
        InteractiveNode.NodeDragged -= Move;
    }



    /// <summary>
    /// Method <c>Clicked</c> makes other nodes' connectors visible, providing the node isn't connected already.
    /// </summary>
    public void Clicked()
    {
        if (_connection == null)
        {
            MakingConnection?.Invoke();
        }

        else
        {
            Debug.Log("Already connected!");
        }
    }
}