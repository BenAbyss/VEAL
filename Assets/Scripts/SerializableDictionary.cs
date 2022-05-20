using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>SerializableDictionary</c> allows for serializable entries of dictionaries.
/// </summary>
[Serializable]
public class SerializableDictionary<Key, Value> : Dictionary<Key, Value>, ISerializationCallbackReceiver
{
    [SerializeField] private List<Key> keys = new List<Key>();
    [SerializeField] private List<Value> values = new List<Value>();
    
    /// <summary>
    /// Method <c>OnBeforeSerialize</c> stores the current dictionary into lists.
    /// </summary>
    public void OnBeforeSerialize()
    {
        keys = new List<Key>(Keys);
        values = new List<Value>(Values);
    }

    /// <summary>
    /// Method <c>OnAfterDeserialize</c> fills the dictionary with the provided serialized lists.
    /// </summary>
    public void OnAfterDeserialize()
    {
        Clear();
        for (int i = 0; i < keys.Count; i++)
        {
            Add(keys[i], values[i]);
        }
    }
}

/// <summary>
/// Class <c>SerializableDictStringToGamObject</c> allows for serializable entries of string-to-GameObject dictionaries.
/// </summary>
[Serializable] public class SerializableDictStringToGamObject : SerializableDictionary<string, GameObject> {}
