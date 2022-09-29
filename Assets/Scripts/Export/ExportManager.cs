using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class ExportManager : SaveSystem
{
    protected override string Subdirectory => "Exported Code";
    protected override string FileType => ".cs";
    private const string TextCulture = "en-US";
    private static readonly TextInfo TextInfo = new CultureInfo(TextCulture, false).TextInfo;

    // this stores the node and a list of output indexes
    private List<(BasicNode, List<int>)> _nodes = new List<(BasicNode, List<int>)>();
    private List<int> _builtNodes = new List<int>();
    private StreamWriter  _stream;

    /// <summary>
    /// Method <c>SetupExport</c> sets up the needed files in the export folder.
    /// </summary>
    public void SetupExport(string folder)
    {
        // get the folder name in the case of none being provided
        if (folder == null)
        {
            DateTime date = DateTime.Now;
            folder = $"{date.Hour}:{date.Minute} {date.Date}-{date.Month}-{date.Year}";
        }
        // ensure the folder name hasn't been used already
        folder = ValidName(folder, file_type: "");
        
        // creates the relevant files within the folder
        CopyFiles(folder);
        var file = File.Create(Path.Combine(Path.Combine(SubPath(), folder), "Main.cs"));
        _stream = new StreamWriter(file.Name, true);
    }

    /// <summary>
    /// Method <c>CopyFiles</c> copies the ExportQueue and NodeImplementation files into the folder for full export.
    /// </summary>
    private void CopyFiles(string folder)
    {
        try
        {
            var orig_loc = Path.Combine(Directory.GetCurrentDirectory(), "Assets/Scripts/Export");
            var end_loc = Path.Combine(SubPath(), folder);
            File.Copy(Path.Combine(orig_loc, "ExportQueue.cs"), 
                Path.Combine(end_loc, "ExportQueue.cs"), true);
            File.Copy(Path.Combine(orig_loc, "NodeImplementation.cs"), 
                Path.Combine(end_loc, "NodeImplementation.cs"), true);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine("ExportQueue or NodeImplementation file missing");
            throw;
        }
        catch (IOException)
        {
            Console.WriteLine("Error in copying file ExportQueue or NodeImplementation");
            throw;
        }
    }



    public void CreateMainFile()
    {
        // go to right screen
        _stream.Write("public class Main\n{\n");
        // call GetNodes
        // create class start, and class variable counters for all AND gates
        // call ExtractPath on each GetNodes return value
        // finish up file
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
        
        return start_nodes;
    }

    /// <summary>
    /// Method <c>ExtractPath</c> calls node extraction on every possible node in the following paths of a given node.
    /// <param name="node_index">The index of the node that starts the path.</param>
    /// </summary>
    private void ExtractPath(int node_index)
    {
        if (_builtNodes.Contains(node_index)) return;
        
        var outputs = 
            _nodes[node_index].Item2.Select(output => NodeToMethodName(_nodes[output].Item1.name)).ToList();
        ExtractNode(node_index, outputs, 1);
        _builtNodes.Add(node_index);
        foreach (var next_index in _nodes[node_index].Item2)
        {
            ExtractPath(next_index);
        }
    }
    
    
    
    /// <summary>
    /// Method <c>ExtractNode</c> extracts a node into string code, and adds it to the stream.
    /// <param name="node_index">The index of the node to extract.</param>
    /// <param name="outputs">A list of the methods that are possible outputs of the node.</param>
    /// <param name="indent_amnt">How many times to indent the code.</param>
    /// </summary>
    private void ExtractNode(int node_index, List<string> outputs, int indent_amnt)
    {
        // the node name is used for the method name for seamless pathing without repeated implementations
        // the return value states what paths to add to the queue, or an empty list for none
        var node = _nodes[node_index].Item1;
        var node_code = "private List<string> " + NodeToMethodName(node.name) + "()\n{\n";
        var outputs_code = "new List<string>{" + string.Join(", ", outputs) + "};";
        indent_amnt++;

        var produced_code = "";
        switch (node.GetNodeType())
        {
            case "Interactive Node" : 
                // create internal ExportQueue
                break;
            case "Decision Node" :
                produced_code = "return NodeImplementation.DecisionImpl(" +
                                $"{((DecisionNode) node).GetPathsTaken()}, {outputs_code});";
                break;
            case "Loop Node" :
                var loop = FindLoop(node.name, node_index, new List<string>());
                var loop_code = "new List<string>{" + string.Join(", ", loop) + "};";
                produced_code = $"return NodeImplementation.LoopImpl(" +
                                $"{((LoopNode) node).LoopCount}, {loop_code}, {outputs_code});";
                break;
            case "Probability Node" :
                produced_code = "return new System.Random().Next(1, 100) <= " +
                                $"{node.name.Replace("%", "")} ? {outputs_code} : new List<string>();";
                break;
            case "AND Gate Node" :
                produced_code = $"return NodeImplementation.ANDGateImpl({node.name}, {outputs_code});";
                break;
        }
        
        node_code += Indent(produced_code, 1) + "\n}\n";
        _stream.Write(Indent(node_code + "\n", indent_amnt));
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
        return str.Replace("\n", "\n" + indent);
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
}