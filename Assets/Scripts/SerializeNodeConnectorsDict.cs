using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Class <c>SerializableDictStringToGamObject</c> allows for serializable entries of string-to-GameObject dictionaries.
/// </summary>
[Serializable]
public class SerializeNodeConnectorsDict : Dictionary<string, GameObject>, ISerializationCallbackReceiver
{
    [SerializeField] private string[] keys = new string[8];
    [SerializeField] private GameObject[] values = new GameObject[8];

    /// <summary>
    /// Method <c>OnBeforeSerialize</c> stores the current dictionary into lists.
    /// </summary>
    public void OnBeforeSerialize()
    {
        int counter = 0;
        foreach(var pair in this)
        {
            keys[counter] = pair.Key;
            values[counter] = pair.Value;
            counter++;
        }
    }

    /// <summary>
    /// Method <c>OnAfterDeserialize</c> fills the dictionary with the provided serialized lists.
    /// </summary>
    public void OnAfterDeserialize()
    {
        Clear();
        
        for (int i = 0; i < 8; i++)
        {
            this[keys[i]] = values[i];
        }
    }

    /// <summary>
    /// Method <c>GetNodeFunction</c> gets the functional NodeConnector object of a given connector.
    /// <param name="key">The key to get the function of.</param>
    /// <returns>The functional NodeConnector object of the connector</returns>
    /// </summary>
    public NodeConnector GetNodeFunction(string key)
    {
        return this[key].GetComponent<NodeConnector>();
    }

    /// <summary>
    /// Method <c>GetNodeFunctions</c> gets all the functional NodeConnector objects.
    /// <returns>The functional NodeConnector objects</returns>
    /// </summary>
    public NodeConnector[] GetNodeFunctions()
    {
        NodeConnector[] functions = new NodeConnector[8];
        int counter = 0;
        foreach (var value in values)
        {
            functions[counter] = value.GetComponent<NodeConnector>();
            counter++;
        }
        return functions;
    }
}
