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
    private const string TextCulture = "en-UK";
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
        var start_nodes = GetNodes();
        var and_gate_var = "private Dictionary<string, int> _andGates = new Dictionary<string, int> {";
        
        // create a dictionary of counters for each AND gate, to register if both paths call them
        foreach (var (node, path) in _nodes)
        {
            if (node is ANDGateNode)
            {
                and_gate_var += "{" + node.name + ", 0}";
            }
        }
        
        _stream.Write("public class Main\n{\n");
        // set up the AND gate implementations
        _stream.Write(Indent(and_gate_var + "};\n \n", 1));
        
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
        
        ExtractNode(_nodes[node_index].Item1, 1);
        _builtNodes.Add(node_index);
        foreach (var next_index in _nodes[node_index].Item2)
        {
            ExtractPath(next_index);
        }
    }
    
    
    
    private void ExtractNode(BasicNode node, int indent_amnt)
    {
        // the return value states whether to continue the path
        var node_code = "private bool " + 
                        TextInfo.ToTitleCase(node.name.ToLower()).Replace(" ", "") + "()\n{\n";
        indent_amnt++;

        var produced_code = "";
        switch (node.GetNodeType())
        {
            case "Interactive Node" : break;
            case "Decision Node" : break;
            case "Loop Node" : break;
            case "Probability Node" :
                produced_code = "return new System.Random().Next(1, 100) <= " +
                                $"{node.name.Replace("%", "")};";
                break;
            case "AND Gate Node" :
                produced_code = $"return NodeImplementation.ANDGateImpl({node.name});";
                break;
        }
        node_code += Indent(produced_code, 1) + "\n}\n";
        
        // have it be a function with the node name as the method name
        // this then allows for seamless pathing between them and avoidance of repeated implementations
        _stream.Write(Indent(node_code + "\n", indent_amnt));
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