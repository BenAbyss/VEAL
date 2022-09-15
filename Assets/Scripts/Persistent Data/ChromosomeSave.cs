using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ChromosomeSave : SaveSystem
{
    public static event Action<Chromosome> LoadChromosome;
    public static event Action TopChromosomeReached;
    public static event Action<string, string> ChromosomeLeft;
    protected override string Subdirectory => "Chromosomes";
    private const string DefaultPath = "MainChromosome";
    
    private string _currentScene = "MainChromosome"; // replaced by save file name if it's a saved project
    private Stack<string> _previousScenes = new Stack<string>();

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        ChromosomeCreationManager.EnterVariable += EnterInternals;
        ChromosomeCreationManager.SaveChromosome += SaveChromosome;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        ChromosomeCreationManager.EnterVariable -= EnterInternals;
        ChromosomeCreationManager.SaveChromosome -= SaveChromosome;
    }



    /// <summary>
    /// Method <c>EnterInternals</c> enters the internals of a struct variable.
    /// <param name="chromosome">The chromosome this struct belongs to.</param>
    /// <param name="variable">The struct variable.</param>
    /// </summary>
    private void EnterInternals(Chromosome chromosome, ChromosomeVariable variable)
    {
        var path = SetupInternalsName(variable);
        variable.UpdateInternals(path);
        SaveChromosome(chromosome, ToPath(chromosome.parentChromosome ?? DefaultPath));
        using (var file_reader = new StreamReader(path))
        {
            var new_chromosome = JsonUtility.FromJson<Chromosome>(file_reader.ReadLine()) ?? new Chromosome();
            new_chromosome.parentChromosome = variable.chrName ?? "MainChromosome";
            ChromosomeCreationManager.ParentChromosome = new_chromosome.parentChromosome;
            _currentScene = ToPath(chromosome.parentChromosome ?? DefaultPath);
            LoadChromosome?.Invoke(new_chromosome);
        }
        
        SwapScenes(path, true);
    }

    
    
    /// <summary>
    /// Method <c>SwapScenes</c> saves and destroys the current scene, before loading the new scene.
    /// <param name="filename">The name of the new scene to load.</param>
    /// <param name="add_scene">Whether to add this new scene to the stack of scenes.</param>
    /// </summary>
    private void SwapScenes(string path, bool add_scene)
    {
        if (add_scene)
        {
            _previousScenes.Push(_currentScene);
        }
        using (var file_reader = new StreamReader(path))
        {
            var new_chromosome = JsonUtility.FromJson<Chromosome>(file_reader.ReadLine());
            if (new_chromosome != null)
            {
                ChromosomeCreationManager.ParentChromosome = new_chromosome.parentChromosome;
            }
            LoadChromosome?.Invoke(new_chromosome);
        }
        _currentScene = path;
    }
    
    /// <summary>
    /// Method <c>PreviousScene</c> saves and destroys the current scene, before loading the previous scene.
    /// </summary>
    public void PreviousScene()
    {
        if (_previousScenes.Count == 0) return;
        if (_previousScenes.Count == 1)
        {
            TopChromosomeReached?.Invoke();
        }
        ChromosomeLeft?.Invoke(_currentScene, null);
        SwapScenes(_previousScenes.Pop(), false);
    }
    
    /// <summary>
    /// Method <c>SetupInternalsName</c> sets up or updates the name of the internals.
    /// <param name="variable">The variable to edit the internals of.</param>
    /// </summary>
    private string SetupInternalsName(ChromosomeVariable variable)
    {
        var path = NewFilePath(variable.chrName ?? "DefaultVariable");
        if (variable.internalsName != null)
        {
            File.Move(variable.internalsName, path);
        }
        else
        {
            File.Create(path).Dispose();
        }
        
        return path;
    }

    /// <summary>
    /// Method <c>SaveChromosome</c> serializes and saves a given chromosome in a given path.
    /// <param name="chromosome">The chromosome to save.</param>
    /// <param name="path">The file path to save it to.</param>
    /// </summary>
    private void SaveChromosome (Chromosome chromosome, string path)
    {
        var json_serialized = JsonUtility.ToJson(chromosome);
        File.WriteAllText(path, json_serialized);
    }
}
