using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using Diag_Debug = System.Diagnostics.Debug;

public class ExportManager : SaveSystem
{
    protected override string Subdirectory => "Exported Code";
    protected override string FileType => ".cs";
    private const string TextCulture = "en-US";
    private static readonly TextInfo TextInfo = new CultureInfo(TextCulture, false).TextInfo;
    public static event Action LoadNodesScene;
    public static event Action<string> LoadInternals;

    // this stores the node and a list of output indexes
    private string _folder;
    private List<(BasicNode, List<int>)> _nodes = new List<(BasicNode, List<int>)>();
    private List<int> _builtNodes = new List<int>();
    private Dictionary<string, StreamWriter>  _stream = new Dictionary<string, StreamWriter>();

    /// <summary>
    /// Method <c>SetupExport</c> sets up the needed files in the export folder.
    /// <param name="folder_name">The folder to export to.</param>
    /// </summary>
    public void SetupExport(string folder_name)
    {
        // get the folder name in the case of none being provided
        if (folder_name == null)
        {
            DateTime date = DateTime.Now;
            folder_name = $"{date.Hour}-{date.Minute} {date.Day}-{date.Month}-{date.Year}";
        }
        // ensure the folder name hasn't been used already
        folder_name = ValidName(folder_name, file_type: "");
        _folder = folder_name;
        Directory.CreateDirectory(Path.Combine(SubPath(), _folder));
        
        // creates the relevant files within the folder
        CreateFile("Main");
        CopyFiles();
    }

    /// <summary>
    /// Method <c>CopyFiles</c> copies the ExportStack and NodeImplementation files into the folder for full export.
    /// </summary>
    private void CopyFiles()
    {
        try
        {
            var orig_loc = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Scripts/Export");
            var end_loc = Path.Combine(SubPath(), _folder);
            File.Copy(Path.Combine(orig_loc, "ExportStack.cs"), 
                Path.Combine(end_loc, "ExportStack.cs"), true);
            File.Copy(Path.Combine(orig_loc, "NodeImplementation.cs"), 
                Path.Combine(end_loc, "NodeImplementation.cs"), true);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("ExportStack or NodeImplementation file missing");
            throw;
        }
        catch (IOException)
        {
            Console.WriteLine("Error in copying file ExportStack or NodeImplementation");
            throw;
        }
    }
    
    /// <summary>
    /// Method <c>CreateFile</c> creates a cs file with an appropriate name and location, and writes the code.
    /// <param name="filename">The file to create.</param>
    /// <param name="node_name">The name of the node the file represents, if it represents an interactive node.</param>
    /// </summary>
    private void CreateFile(string filename, string node_name="Main")
    {
        //create the file
        var file = File.Create(Path.Combine(Path.Combine(SubPath(), _folder), filename + ".cs"));
        _stream[filename] = new StreamWriter(file);
        _stream[filename].Write("public class " + filename + " : NodeContents\n{\n");
        
        // go to right screen
        StartCoroutine(filename == "Main" ? NodesSceneCall() : InternalSceneCall(node_name));

        // extract all nodes' information
        var nodes = GetNodes();
        foreach (var node in nodes)
        {
            ExtractPath(node_name, node);
        }
        _stream[filename].Write("\n}");
        _stream[filename].Close();
    }

    /// <summary>
    /// Method <c>GetNodes</c> extracts all nodes to the class variable, labelling their output connections.
    /// <returns>A list of indexes of start nodes(those with no inputs).</returns>
    /// </summary>
    private List<int> GetNodes()
    {
        var nodes = FindObjectsOfType<BasicNode>();
        var start_nodes = new List<int>();

        for (var i = 0; i < nodes.Length; i++)
        {
            var conns = nodes[i].GetNodeConnectors();
            
            if (conns.GetUsedConnectors(false, true).Count == 0)
            {
                start_nodes.Add(i);
            }

            var output_nodes = conns.GetUsedConnectors(true, false)
                .Select(out_conn => Array.IndexOf(nodes, 
                    out_conn.connectorGroup.GetConnectedNodeFunc())).ToList();
            _nodes.Add((nodes[i], output_nodes));
        }

        foreach (var n in _nodes)
        {
            var list = "";
            foreach (var i in n.Item2)
            {
                list += $"{i}, ";
            }
            Debug.Log($"Node {n.Item1.name} has outputs {list}");
        }
        return start_nodes;
    }

    /// <summary>
    /// Method <c>ExtractPath</c> calls node extraction on every possible node in the following paths of a given node.
    /// <param name="file">The file to write the code into.</param>
    /// <param name="node_index">The index of the node that starts the path.</param>
    /// </summary>
    private void ExtractPath(string file, int node_index)
    {
        if (_builtNodes.Contains(node_index)) return;
        
        var outputs = 
            _nodes[node_index].Item2.Select(output => NodeToMethodName(_nodes[output].Item1.name)).ToList();
        ExtractNode(file, node_index, outputs, 1);
        _builtNodes.Add(node_index);
        foreach (var next_index in _nodes[node_index].Item2)
        {
            ExtractPath(file, next_index);
        }
    }
    
    
    
    /// <summary>
    /// Method <c>ExtractNode</c> extracts a node into string code, and adds it to the stream.
    /// <param name="file">The file to write the code into.</param>
    /// <param name="node_index">The index of the node to extract.</param>
    /// <param name="outputs">A list of the methods that are possible outputs of the node.</param>
    /// <param name="indent_amnt">How many times to indent the code.</param>
    /// </summary>
    private void ExtractNode(string file, int node_index, IEnumerable<string> outputs, int indent_amnt)
    {
        // the node name is used for the method name for seamless pathing without repeated implementations
        // the return value states what paths to add to the queue, or an empty list for none
        var node = _nodes[node_index].Item1;
        var node_code = "private List<string> " + NodeToMethodName(node.name) + "()\n{\n";
        var outputs_code = "new List<string>{" + string.Join(", ", outputs) + "}";
        indent_amnt++;

        var produced_code = "";
        switch (node.GetNodeType())
        {
            case "Interactive Node" : 
                CreateFile(ValidName(node.name, file_type: ".cs"), node.name);
                produced_code = $"{node.name}.RunStack.CallStack();"; 
                break;
            
            case "Decision Node" :
                produced_code = "var result = NodeImplementation.DecisionImpl(" +
                                $"{((DecisionNode) node).GetPathsTaken()}, {outputs_code}, this);\n" +
                                "AppendNewNodes(result);";
                break;
            
            case "Loop Node" :
                var loop = FindLoop(node.name, node_index, new List<string>());
                var loop_code = "new List<string>{" + string.Join(", ", loop) + "};";
                produced_code = "var result = NodeImplementation.LoopImpl(" +
                                $"{node.name}, {((LoopNode) node).LoopCount}, {loop_code}, {outputs_code}, this);\n" +
                                "AppendNewNodes(result);";
                break;
            
            case "Probability Node" :
                produced_code = "var result = NodeImplementation.ProbabilityImpl(" +
                                $"{node.name.Replace("%", "")}, {outputs_code}, this);\n" +
                                "AppendNewNodes(result);";
                break;
            
            case "AND Gate Node" :
                produced_code = $"var result = NodeImplementation.ANDGateImpl({node.name}, {outputs_code}, this);\n" +
                                "AppendNewNodes(result);";
                break;
        }
        
        node_code += Indent(produced_code, 1) + "\n}\n";
        _stream[file].Write(Indent(node_code + "\n", indent_amnt));
    }

    /// <summary>
    /// Method <c>FindLoop</c> finds the entire path encapsulated within a loop nodes' loop through recursion.
    /// <param name="target">The name of the loop node, to know when it's end is found.</param>
    /// <param name="start_index">The index of the node to start the path checks at.</param>
    /// <param name="path">The currently built up path.</param>
    /// <returns>An ordered list of the path.</returns>
    /// </summary>
    private List<string> FindLoop(string target, int start_index, List<string> path)
    {
        path.Add(NodeToMethodName(_nodes[start_index].Item1.name));
        foreach (var output in _nodes[start_index].Item2)
        {
            var result = FindLoop(target, output, path);
            // if there's the loop node at any point in the path, return the path
            if (_nodes[output].Item1.name == target || result != null)
            {
                path.RemoveAt(path.Count - 1);
                return path;
            }
        }

        // if the loop isn't in this path, return null. This happens for paths called post-loop
        return null;
    }

    /// <summary>
    /// Method <c>Indent</c> appropriately indents a string, ready for code insertion.
    /// <param name="str">The string to indent.</param>
    /// <param name="indent_amnt">How many times to indent it.</param>
    /// <returns>The indented text.</returns>
    /// </summary>
    private string Indent(string str, int indent_amnt)
    {
        var indent = "";
        for (var i = 0; i < indent_amnt; i++) indent += "   ";
        return indent + str.Replace("\n", "\n" + indent);
    }

    /// <summary>
    /// Method <c>NodeToMethodName</c> transforms a nodes' name into a method name.
    /// <param name="node_name">The node name to translate.</param>
    /// <returns>The resultant method name.</returns>
    /// </summary>
    private string NodeToMethodName(string node_name)
    {
        return TextInfo.ToTitleCase(node_name.ToLower()).Replace(" ", "");
    }
    
    /// <summary>
    /// Method <c>CheckOutputsAreProb</c> checks if all the output paths of a node are probability nodes.
    /// <param name="node">The node to check the outputs of.</param>
    /// </summary>
    private bool CheckOutputsAreProb(BasicNode node)
    {
        var all_outputs = true;
        foreach (NodeConnector conn in node.GetNodeConnectors().GetUsedConnectors(true, false))
        {
            all_outputs &= conn.GetConnectionTo().GetNodesType() == "Probability Node";
        }

        return all_outputs;
    }
    
    
    
    /// <summary>
    /// Method <c>NodesSceneCall</c> loads the main node scene for analysis.
    /// </summary>
    private IEnumerator NodesSceneCall()
    {
        LoadNodesScene?.Invoke();
        yield return new WaitForSecondsRealtime(0.1f);
    }
    
    /// <summary>
    /// Method <c>NodesSceneCall</c> loads the appropriate nodes' internal scene for analysis.
    /// <param name="internals_name">The name of the internals screen to load.</param>
    /// </summary>
    private IEnumerator InternalSceneCall(string internals_name)
    {
        LoadInternals?.Invoke(internals_name);
        yield return new WaitForSecondsRealtime(0.1f);
    }
}