using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    protected virtual string Subdirectory => "General";

    public void Start()
    {
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "SaveFiles"));
        Directory.CreateDirectory(SubPath());
    }
    
    /// <summary>
    /// Method <c>DeleteFile</c> deletes a specified file.
    /// <param name="file">The file to delete.</param>
    /// </summary>
    protected void DeleteFile(string file)
    {
        try
        {
            File.Delete(ToPath(file));
        }
        catch (FileNotFoundException) { }
    }
    
    
    
    /// <summary>
    /// Method <c>ValidName</c> gets a valid file name from a given name, by adding an appropriate counter suffix.
    /// <param name="name">The base name of the file.</param>
    /// <returns>The adjusted valid name for the file.</returns>
    /// </summary>
    protected string ValidName(string name)
    {
        name = name.Trim();
        var files = new List<string>(Directory.GetFiles(SubPath(), @"*.sav"));

        if (files.Contains(name))
        {
            var suffix_val = 1;
            while (files.Contains(name + " (" + suffix_val + ")"))
            {
                suffix_val++;
            }

            name = name + " (" + suffix_val + ")";
        }

        return name;
    }
    
    /// <summary>
    /// Method <c>ToPath</c> converts a given file name to a save file equivalent.
    /// <param name="file">The name of the file.</param>
    /// <returns>The path name for the file.</returns>
    /// </summary>
    protected string ToPath(string file)
    {
        return Path.Combine(SubPath(), file + ".sav");
    }

    /// <summary>
    /// Method <c>SubPath</c> gets the complete persistent data path of the subdirectory.
    /// <returns>The persistent data path of the subdirectory.</returns>
    /// </summary>
    private string SubPath()
    {
        return Path.Combine(Application.persistentDataPath, Path.Combine("SaveFiles", Subdirectory));
    }
}