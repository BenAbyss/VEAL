using System;
using System.IO;
using System.Reflection;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ToolbarManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown fileDropdown;
    [SerializeField] private TMP_Dropdown navigateDropdown;
    [SerializeField] private TMP_Dropdown settingsDropdown;
    
    [SerializeField] private SaveAsManager saveManager;
    [SerializeField] private LoadPopupManager loadManager;
    [SerializeField] private ExportManager exportManager;
    
    [SerializeField] [CanBeNull] private SceneSave nodeSave;
    [SerializeField] [CanBeNull] private ChromosomeCreationManager chromosomeSave;
    
    public static string ConstructionType = "Evolutionary Action";
    private string _lastSave;

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        SaveAsManager.SaveFile += SaveWithName;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        SaveAsManager.SaveFile -= SaveWithName;
    }
    
    
    
    /// <summary>
    /// Method <c>FilePressed</c> calls the appropriate function for the selected feature.
    /// <param name="pos">The position of the chosen option in the dropdown.</param>
    /// </summary>
    public void FilePressed(int pos)
    {
        var method_name = fileDropdown.options[pos].text.Replace(" ", "").Replace(".", "");
        var method = GetType().GetMethod(method_name, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(this, null);
    }
    
    /// <summary>
    /// Method <c>NavigatePressed</c> calls the appropriate function for the selected feature.
    /// <param name="pos">The position of the chosen option in the dropdown.</param>
    /// </summary>
    public void NavigatePressed(int pos)
    {
        var method_name = navigateDropdown.options[pos].text.Replace(" ", "").Replace(".", "");
        var method = GetType().GetMethod(method_name, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(this, null);
    }
    
    /// <summary>
    /// Method <c>SettingsPressed</c> calls the appropriate function for the selected feature.
    /// <param name="pos">The position of the chosen option in the dropdown.</param>
    /// </summary>
    public void SettingsPressed(int pos)
    {
        var method_name = settingsDropdown.options[pos].text.Replace(" ", "").Replace(".", "");
        var method = GetType().GetMethod(method_name, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(this, null);
    }
    
    
    
    /// <summary>
    /// Method <c>Save</c> saves the current work to it's designated file, or calls Save As if it has none.
    /// </summary>
    private void Save()
    {
        if (_lastSave == null)
        {
            SaveAs();
        }
        else
        {
            SaveWithName(_lastSave);
        }
    }

    /// <summary>
    /// Method <c>SaveAs</c> prompts the user to provide a file name for the current work, and saves it there.
    /// </summary>
    private void SaveAs()
    {
        saveManager.ChangeActivity(true);
    }

    /// <summary>
    /// Method <c>Load</c> loads a previously saved piece of work.
    /// </summary>
    private void Load()
    {
        loadManager.ChangeActivity(true);
        var folder_path = Path.Combine(Application.persistentDataPath,
            Path.Combine("SaveFiles", Path.Combine(nodeSave != null ? "Scenes" : "Chromosomes", ConstructionType)));
        loadManager.LoadOptions(folder_path);
    }

    /// <summary>
    /// Method <c>Export</c> exports the created system into code.
    /// </summary>
    private void Export()
    {
        exportManager.SetupExport(_lastSave);
    }

    /// <summary>
    /// Method <c>NewChromosome</c> creates a new empty chromosome and opens it in the chromosome creation screen.
    /// </summary>
    private void NewChromosome()
    {
        ConstructionType = "Chromosome";
        Debug.Log("Feature not implemented!");
    }

    /// <summary>
    /// Method <c>NewEvolutionaryAlgorithm</c> creates a new empty evolutionary algorithm and opens it in the
    /// node pathing screen.
    /// </summary>
    private void NewEvolutionaryAlgorithm()
    {
        ConstructionType = "Evolutionary Action";
        Debug.Log("Feature not implemented!");
    }

    /// <summary>
    /// Method <c>GoToTop</c> goes to the top node path or chromosome in the tree of the current piece of work.
    /// </summary>
    private void GoToTop()
    {
        Debug.Log("Feature not implemented!");
    }

    /// <summary>
    /// Method <c>Merge</c> merges an evolutionary algorithm node path with a chromosome.
    /// </summary>
    private void Merge()
    {
        Debug.Log("Feature not implemented!");
    }

    /// <summary>
    /// Method <c>SystemSettings</c> allows users to customise the settings of the system.
    /// </summary>
    private void SystemSettings()
    {
        Debug.Log("Feature not implemented!");
    }

    /// <summary>
    /// Method <c>Help</c> provides a brief help guide on using the system.
    /// </summary>
    private void Help()
    {
        Debug.Log("Feature not implemented!");
    }



    /// <summary>
    /// Method <c>SaveWithName</c> serializes and saves the file with the given name.
    /// <param name="filename">The name of the file to store the serialization within.</param>
    /// </summary>
    private void SaveWithName(string filename)
    {
        if (nodeSave != null)
        {
            nodeSave.Save(filename);
        }
        else
        {
            chromosomeSave?.Save(filename, ConstructionType);
        }
    }
}