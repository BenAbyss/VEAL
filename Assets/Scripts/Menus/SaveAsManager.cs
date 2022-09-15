using System;
using TMPro;
using UnityEngine;

public class SaveAsManager : MenuManager
{
    public static event Action<string> SaveFile;
    [SerializeField] private TMP_InputField input;

    /// <summary>
    /// Method <c>Start</c> disables the menu
    /// </summary>
    public void Start()
    {
        CloseMenu();
    }
    
    /// <summary>
    /// Method <c>Save</c> makes an action invocation to save the file with the currently inputted name.
    /// </summary>
    public void Save()
    {
        SaveFile?.Invoke(input.text);
    }

    /// <summary>
    /// Method <c>Cancel</c> closes the menu.
    /// </summary>
    public void Cancel()
    {
        input.text = "";
        CloseMenu();
    }
    
    /// <summary>
    /// Method <c>InputChanged</c> removes any full stops from the input
    /// <param name="text">The current input text.</param>.
    /// </summary>
    public void InputChanged(string text)
    {
        input.text = text.Replace(".", "");
    }
}