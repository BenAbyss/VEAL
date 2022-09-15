using System;
using UnityEngine;
using UnityEngine.UI;

public class LoadPopupFileManager : MonoBehaviour
{
    
    public static Action<string> FileSelected;
    private Button _btn;
    private string _filename;

    /// <summary>
    /// Method <c>Start</c> finds the button element
    /// </summary>
    public void Start()
    {
        _btn = GetComponent<Button>();
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        LoadPopupManager.ChangeHighlight += SetHighlight;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        LoadPopupManager.ChangeHighlight -= SetHighlight;
    }
    
    
    
    /// <summary>
    /// Method <c>SetFileName</c> sets the buttons file name and text.
    /// </summary>
    public void SetFileName(string new_name)
    {
        _filename = new_name;
        GetComponentInChildren<Text>().text = new_name;
    }

    /// <summary>
    /// Method <c>OnClick</c> invokes that the file has been selected.
    /// </summary>
    public void OnClick()
    {
        FileSelected?.Invoke(_filename);
    }

    /// <summary>
    /// Method <c>SetHighlight</c> sets the normal colour of the selection button to match the selected colour,
    /// and the normal colour of all other buttons to white. This avoids clicking away from the buttons hiding
    /// which file is highlighted.
    /// <param name="selection">The selected file.</param>
    /// </summary>
    private void SetHighlight(string selection)
    {
        var color_block = _btn.colors;
        color_block.normalColor = selection == _filename ? 
            new Color32(132, 226, 236, 255) : new Color32(255, 255, 255, 255);
        _btn.colors = color_block;
    }
}