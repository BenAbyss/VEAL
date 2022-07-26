using System;
using System.IO;
using UnityEngine;

public class ChromosomeSave : SaveSystem
{
    public static event Action<Chromosome> LoadChromosome;
    protected override string Subdirectory => "Chromosomes";
    private const string DefaultPath = "MainChromosome";

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        ChromosomeCreationManager.EnterVariable += EnterInternals;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        ChromosomeCreationManager.EnterVariable -= EnterInternals;
    }



    private void EnterInternals(Chromosome chromosome, ChromosomeVariable variable)
    {
        var path = SetupInternalsName(variable);
        SaveInternals(chromosome, chromosome.parentChromosome ?? DefaultPath);
        using (var file_reader = new StreamReader(path))
        {
            LoadChromosome?.Invoke(JsonUtility.FromJson<Chromosome>(file_reader.ReadLine()));
        }
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

    private void SaveInternals(Chromosome chromosome, string path)
    {
        var json_serialized = JsonUtility.ToJson(chromosome);
        File.WriteAllText(path, json_serialized);
    }
}
