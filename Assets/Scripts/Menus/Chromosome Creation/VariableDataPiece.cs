using TMPro;
using UnityEngine;

public class VariableDataPiece : MonoBehaviour
{
    private TMP_InputField _nameInput;
    private TMP_Dropdown _typeInput;
    private TextMeshProUGUI _limitsText;

    public void Start()
    {
        _nameInput = GetComponentInChildren<TMP_InputField>();
        _typeInput = GetComponentInChildren<TMP_Dropdown>();
        _limitsText = GameObject.Find("LimitsText").GetComponent<TextMeshProUGUI>();
    }
}