using TMPro;
using UnityEngine;

public class DecisionNodePathNameSubsection : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI posText;
    private int _pos;
    private DecisionNodeSpecsPanelManager _manager;

    public void SetManager(DecisionNodeSpecsPanelManager new_manager)
    {
        _manager = new_manager;
    }
    
    /// <summary>
    /// Method <c>SetPos</c> sets the position of the path subsection.
    /// <param name="new_pos">The path's position.</param>
    /// </summary>
    public void SetPos(int new_pos)
    {
        _pos = new_pos;
        posText.text = $"{new_pos}.";
    }

    /// <summary>
    /// Method <c>GetPos</c> gets the position of the path subsection.
    /// </summary>
    public int GetPos()
    {
        return _pos;
    }

    /// <summary>
    /// Method <c>SetName</c> sets the name of the path.
    /// <param name="new_name">The path's name.</param>
    /// </summary>
    public void SetName(string new_name)
    {
        inputField.text = new_name;
    }
    
    /// <summary>
    /// Method <c>GetName</c> gets the name of the path.
    /// </summary>
    public string GetName()
    {
        return inputField.text;
    }

    /// <summary>
    /// Method <c>EditStarted</c> marks the inputs as typing.
    /// </summary>
    public void EditStarted()
    {
        InputManager.isTyping = true;
    }
    
    /// <summary>
    /// Method <c>NewNameEntered</c> calls the manager to set the paths new name.
    /// <param name="new_name">The path's new name.</param>
    /// </summary>
    public void NewNameEntered(string new_name)
    {
        _manager.SetPathName(new_name, _pos-1);
        InputManager.isTyping = false;
    }

    /// <summary>
    /// Method <c>Remove</c> removes and destroys the path
    /// </summary>
    public void Remove()
    {
        _manager.RemovePath(_pos);
        Destroy(gameObject);
    }
}