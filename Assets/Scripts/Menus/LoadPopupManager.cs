using System;
using System.IO;
using UnityEngine;

public class LoadPopupManager : MenuManager
{
    public static event Action<string> LoadFile;
    public static event Action<string> ChangeHighlight;
    [SerializeField] private GameObject scroller;
    [SerializeField] private GameObject filePrefab;
    private string _currFile;

    /// <summary>
    /// Method <c>Start</c> disables the menu
    /// </summary>
    public void Start()
    {
        CloseMenu();
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        base.OnEnable();
        LoadPopupFileManager.FileSelected += FileSelected;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        base.OnDisable();
        LoadPopupFileManager.FileSelected -= FileSelected;
    }
    
    
    
    /// <summary>
    /// Method <c>LoadOptions</c> loads up buttons for all the valid file options.
    /// <param name="folder">The folder to collect options from.</param>
    /// </summary>
    public void LoadOptions(string folder)
    {
        var options = Directory.GetFiles(folder);
        foreach (var option in options)
        {
            var new_btn = Instantiate(filePrefab, scroller.transform, false);
            new_btn.GetComponent<LoadPopupFileManager>().SetFileName(option);
        }
    }

    /// <summary>
    /// Method <c>FileSelected</c> changes the current selected file.
    /// <param name="new_file">The new selected file.</param>
    /// </summary>
    private void FileSelected(string new_file)
    {
        _currFile = new_file;
        ChangeHighlight?.Invoke(new_file);
    }
    
    
    
    /// <summary>
    /// Method <c>Load</c> loads the currently selected file.
    /// </summary>
    public void Load()
    {
        LoadFile?.Invoke(_currFile);
    }

    /// <summary>
    /// Method <c>Cancel</c> closes the menu.
    /// </summary>
    public void Cancel()
    {
        CloseMenu();
    }
}