using System;
using TMPro;
using UnityEngine;

public class InvalidValueDataPiece : MonoBehaviour
{
    public static event Action<string> DeletedInvalidValue;
    [SerializeField] private TextMeshProUGUI value;
    
    /// <summary>
    /// Method <c>Delete</c> calls it's deletion before deleting the game object.
    /// </summary>
    public void Delete()
    {
        DeletedInvalidValue?.Invoke(value.text);
        Destroy(gameObject);
    }
}