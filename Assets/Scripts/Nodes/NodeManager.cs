using System;
using System.Collections;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static event Action<Midpoint> MidpointCreated;
    [SerializeField] private GameObject midpointPrefab;
    
    // ReSharper disable Unity.PerformanceAnalysis
    /// <summary>
    /// Method <c>CreateMidpoint</c> instantiates a midpoint. 
    /// </summary>
    /// This is done as an IEnumerator to allow for a brief delay to let the midpoint set up.
    public IEnumerator CreateMidpoint()
    {
        var coords = InputManager.GetMouseCoords();
        coords.z = -1;
        var midpoint = Instantiate(midpointPrefab, coords, Quaternion.identity,
            GameObject.FindGameObjectWithTag("Canvas").transform);
        yield return new WaitForSeconds(0.001F);
        var midpoint_func = midpoint.GetComponentInChildren<Midpoint>();
        midpoint.name = "Midpoint " + midpoint_func.nodeId;
        MidpointCreated?.Invoke(midpoint_func);
        midpoint_func.ContinueConnection();
    }
}