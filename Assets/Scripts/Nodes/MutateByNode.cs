using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MutateByNode : InteractiveNode
{
    protected override string NodeType => "MutateBy";
    [SerializeField] private TMP_InputField percentInput;
    [SerializeField] private TMP_Dropdown varSelection;
    public List<string> variables;
    
    /// <summary>
    /// Method <c>Start</c> disables hidden elements.
    /// </summary>
    public new void Start()
    {
        var side_menu_func = sideMenu.GetComponent<NodeSideMenu>();
        var selection = varSelection.GetComponent<NodeFollowingObject>();
        var percent = percentInput.GetComponent<NodeFollowingObject>();
        var position = transform.position;
        
        side_menu_func.SetPositioning(position);
        side_menu_func.SetNodeId(nodeId);
        selection.SetPositioning(position);
        selection.SetNodeId(nodeId);
        percent.SetPositioning(position);
        percent.SetNodeId(nodeId);
        NodeConnectors.SetupConnectors(position);
        NodeConnectors.SetNodeType(NodeType);
        sideMenu.SetActive(false);
    }
    
    /// <summary>
    /// Method <c>Awake</c> sets the node counter and finds relevant components.
    /// </summary>
    public new void Awake()
    {
        NodesCounter += 1;
        nodeId = NodesCounter;
        NodeConnectors = GetComponent<NodeConnectors>();
        NodeConnectors.SetNodeId(nodeId);
        NodeConnectors.SetOutputLimit(OutputLimit);
        UpdateVarSelect();
    }

    /// <summary>
    /// Method <c>UpdateVarSelect</c> adds all the possible variable selection values.
    /// </summary>
    public void UpdateVarSelect()
    {
        varSelection.ClearOptions();
        if (!variables.Contains("All")) variables.Insert(0, "All");
        varSelection.AddOptions(variables);
    }

    /// <summary>
    /// Method <c>PercentUpdated</c> ensures the percent text is only a % string.
    /// <param name="text">The current input text.</param>
    /// </summary>
    public void PercentUpdated(string text)
    {
        percentInput.text = Regex.Replace(text, "[^0-9]", "") + "%";
    }
}
