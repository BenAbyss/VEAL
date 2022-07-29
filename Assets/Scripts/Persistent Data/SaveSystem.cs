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
    /// Method <c>NewFilePath</c> gets a unique and valid file name for within a path from a given name.
    /// <param name="filename">The base name of the file.</param>
    /// <returns>The total path for the file.</returns>
    /// </summary>
    protected string NewFilePath(string filename)
    {
        return ToPath(ValidName(filename));
    }
    
    /// <summary>
    /// Method <c>ValidName</c> gets a valid file name from a given name, by adding an appropriate counter suffix.
    /// <param name="filename">The base name of the file.</param>
    /// <param name="folder">The folder within the subpath that it's stored in.</param>
    /// <returns>The adjusted valid name for the file.</returns>
    /// </summary>
    protected string ValidName(string filename, string folder="")
    {
        filename = filename.Trim();
        if (filename.EndsWith(".sav"))
        {
            filename = filename.Substring(0, filename.Length - 4);
        }
        
        var files = new List<string>(Directory.GetFiles(Path.Combine(SubPath(), folder), @"*.sav"));

        if (files.Contains(filename + ".sav"))
        {
            var suffix_val = 1;
            while (files.Contains(filename + " (" + suffix_val + ")"))
            {
                suffix_val++;
            }

            filename = filename + " (" + suffix_val + ")";
        }

        return filename;
    }
    
    /// <summary>
    /// Method <c>ToPath</c> converts a given file name to a save file equivalent.
    /// <param name="file">The name of the file.</param>
    /// <returns>The path name for the file.</returns>
    /// </summary>
    protected string ToPath(string file)
    {
        // avoid repeating '.sav' endings
        if (file.EndsWith(".sav"))
        {
            file = file.Substring(0, file.Length - 4);
        }
        return Path.Combine(SubPath(), file + ".sav");
    }

    /// <summary>
    /// Method <c>SubPath</c> gets the complete persistent data path of the subdirectory.
    /// <returns>The persistent data path of the subdirectory.</returns>
    /// </summary>
    protected string SubPath()
    {
        return Path.Combine(Application.persistentDataPath, Path.Combine("SaveFiles", Subdirectory));
    }
    
    /// <summary>
    /// Method <c>ToFolderPath</c> gets the complete persistent data path of the subdirectory within a given folder.
    /// <param name="folder">The name of the folder to be stored in.</param>
    /// <param name="file">The name of the file.</param>
    /// <returns>The persistent data path of the subdirectory.</returns>
    /// </summary>
    protected string ToFolderPath(string folder, string file)
    {
        // avoid repeating '.sav' endings
        if (file.EndsWith(".sav"))
        {
            file = file.Substring(0, file.Length - 4);
        }
        
        // create the folder if it isn't already created
        var folder_path = Path.Combine(Application.persistentDataPath,
            Path.Combine("SaveFiles", Path.Combine(Subdirectory, folder)));
        if (!Directory.Exists(folder_path))
        {
            Directory.CreateDirectory(folder_path);
        }
        
        return Path.Combine(folder_path, file + ".sav");
    }
}