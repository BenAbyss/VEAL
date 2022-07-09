using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>SerializableDictStringToGamObject</c> allows for serializable entries of string-to-GameObject dictionaries.
/// </summary>
[Serializable]
public class SerializableStringGameObjDict : Dictionary<string, GameObject>, ISerializationCallbackReceiver
{
    [SerializeField] protected string[] keys = new string[8];
    [SerializeField] protected GameObject[] values = new GameObject[8];

    /// <summary>
    /// Method <c>OnBeforeSerialize</c> stores the current dictionary into lists.
    /// </summary>
    public void OnBeforeSerialize()
    {
        var counter = 0;
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
        
        for (var i = 0; i < keys.Length; i++)
        {
            this[keys[i]] = values[i];
        }
    }
}