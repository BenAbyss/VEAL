using System;
using TMPro;
using UnityEngine;

public class InvalidValueDataPiece : MonoBehaviour
{
    public static event Action<string> DeletedInvalidValue;
    [SerializeField] private TextMeshProUGUI value;
    
    public void Delete()
    {
        DeletedInvalidValue?.Invoke(value.text);
        Destroy(gameObject);
    }
}