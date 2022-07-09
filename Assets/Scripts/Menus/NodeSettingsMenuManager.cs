using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NodeSettingsMenuManager : MenuManager
{
    [SerializeField] private GameObject colourSegment;
    [SerializeField] private Image colourSample;
    [SerializeField] private GameObject specDefaultSegment;

    private InteractiveNode _node;
    private GameObject _activeSpecsPanel;
    private Dictionary<string, GameObject> _specsPanels;
    
    private Slider[] _colourSliders;
    private TextMeshProUGUI[] _colourValues;
    private TMP_InputField _textField;

    /// <summary>
    /// Method <c>Start</c> closes the menu.
    /// </summary>
    private void Start()
    {
        CloseMenu();
        _colourSliders = colourSegment.GetComponentsInChildren<Slider>();
        _colourValues = FindObjectsOfType<TextMeshProUGUI>()
            .ToList().FindAll( x=>x.name.EndsWith("Value"))
            .OrderByDescending(x=>x.name).ToArray();
        _textField = GetComponentInChildren<TMP_InputField>();
        SetupSpecsPanels();
        specDefaultSegment.SetActive(false);
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        base.OnEnable();
        NodeSideMenu.Editing += ToggleActive;
        InteractiveNode.NodeSelected += ChangeNode;
        InteractiveNodeSpecsPanelManager.HideMenus += CloseMenu;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        base.OnDisable();
        NodeSideMenu.Editing -= ToggleActive;
        InteractiveNode.NodeSelected -= ChangeNode;
        InteractiveNodeSpecsPanelManager.HideMenus -= CloseMenu;
    }

    /// <summary>
    /// Method <c>SetupSpecsPanels</c> loads and stores all the specific panels.
    /// </summary>
    private void SetupSpecsPanels()
    {
        _specsPanels = new Dictionary<string, GameObject>();
        GameObject panel;
        GameObject prefab;
        
        foreach (var node_type in new []{"Interactive", "Decision", "Probability", "Loop", "AND"})
        {
            prefab = Resources.Load($"Prefabs/UI/Node Settings/Node Settings Panels/{node_type}SpecsPanel") 
                as GameObject;
            panel = Instantiate(prefab, transform, true);
            panel.transform.position = specDefaultSegment.transform.position;
            panel.SetActive(false);
            _specsPanels[node_type] = panel;
        }
    }
    

    
    /// <summary>
    /// Method <c>ChangeNode</c> changes the node to the new node object.
    /// <param name="id_selected">The id of the selected node.</param>
    /// <param name="new_pos">The new position of the selected node.</param>
    /// <param name="game_obj">The selected node game object.</param>
    /// </summary>
    private void ChangeNode(int id_selected, Vector3 new_pos, GameObject game_obj)
    {
        if (_node == null) return;
        
        if (id_selected != _node.nodeId)
        {
            ChangeNode(game_obj.GetComponent<InteractiveNode>());
        }
    }

    /// <summary>
    /// Method <c>ChangeNode</c> updates the menu for the new node.
    /// <param name="new_node">The selected node.</param>
    /// </summary>
    private void ChangeNode(InteractiveNode new_node)
    {
        _node = new_node;
        UpdateColour(_node.GetComponent<SpriteRenderer>().color);
        GameObject.Find("NodeTitle").GetComponent<TextMeshProUGUI>().text = _node.name;
        LoadSpecificPanel();
        _activeSpecsPanel.GetComponent<NodeSpecsPanelManager>().ChangeNode(new_node);
    }
    
    /// <summary>
    /// Method <c>ToggleActive</c> toggles the active state of the menu.
    /// <param name="new_node">The node the menu is working for.</param>
    /// </summary>
    private void ToggleActive(InteractiveNode new_node)
    {
        base.ToggleActive();
        ChangeNode(new_node);
    }

    /// <summary>
    /// Method <c>LoadSpecificPanel</c> disables the current specification panel and loads the new one.
    /// </summary>
    private void LoadSpecificPanel()
    {
        if (_activeSpecsPanel != null)
        {
            _activeSpecsPanel.SetActive(false);
        }
        _activeSpecsPanel = _specsPanels[_node.GetNodeType().Split(' ')[0]];
        _activeSpecsPanel.SetActive(true);
    }



    /// <summary>
    /// Method <c>ApplyColour</c> applies the chosen colour to the current node.
    /// </summary>
    public void ApplyColour()
    {
        _node.ChangeColour(colourSample.color);
    }
    
    /// <summary>
    /// Method <c>OnColourAdjusted</c> changes the colour based on the values entered,
    /// and updates text boxes to express it's value in both decimal and hexadecimal.
    /// </summary>
    public void OnColourAdjusted()
    {
        if (_colourSliders.Length == 0) return;
        
        colourSample.color = new Color(_colourSliders[0].value/255, _colourSliders[1].value/255, 
            _colourSliders[2].value/255);
        for(var i = 0; i < _colourSliders.Length; i++)
        {
            var slider_val = (int) _colourSliders[i].value;
            _colourValues[i].text = $"{slider_val} ({slider_val:X2})";
        }
    }

    /// <summary>
    /// Method <c>ColourHexSelected</c> marks inputs as typing for the input manager.
    /// </summary>
    public void ColourHexSelected()
    {
        InputManager.isTyping = true;
    }
    
    /// <summary>
    /// Method <c>ColourHexInput</c> ensures only valid hex inputs are entered into the hex input field.
    /// </summary>
    public void ColourHexInput()
    {
        var text_val = _textField.text;
        text_val = "#" + Regex.Replace(text_val, "[^0-9ABCDEFabcdef]", "").ToUpper();
        
        _textField.text = text_val.Substring(0, Math.Min(text_val.Length, 7));
    }
    
    /// <summary>
    /// Method <c>OnColourHexAdjusted</c> converts the inputted hex value to decimal, and calls OnColourAdjusted.
    /// </summary>
    public void OnColourHexAdjusted()
    {
        InputManager.isTyping = false;
        for (var i = 0; i < 3; i++)
        {
            _colourSliders[i].value = int.Parse(_textField.text.Substring(1 + i * 2, 2),
                System.Globalization.NumberStyles.HexNumber);
        }
        OnColourAdjusted();
    }

    /// <summary>
    /// Method <c>UpdateColour</c> updates the current sample colour, as well as the input methods' current values.
    /// <param name="colour">The new colour for the sample.</param>
    /// </summary>
    private void UpdateColour(Color colour)
    {
        if (!IsActive) return;
        
        _textField.text = $"#{((int) (colour.r*255)).ToString("X2")}" +
                          $"{((int)(colour.g*255)).ToString("X2")}{((int)(colour.b*255)).ToString("X2")}";
        OnColourHexAdjusted();
    }
}