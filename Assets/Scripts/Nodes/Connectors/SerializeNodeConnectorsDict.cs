using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// Class <c>SerializableDictStringToGamObject</c> allows for serializable entries of string-to-Node dictionaries.
/// </summary>
[Serializable]
public class SerializeNodeConnectorsDict : SerializableStringGameObjDict
{
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
        var functions = new NodeConnector[keys.Length];
        var counter = 0;
        foreach (var value in values)
        {
            functions[counter] = value.GetComponent<NodeConnector>();
            counter++;
        }
        return functions;
    }
}