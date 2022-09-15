using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeCreatorMenuManager : MenuManager
{
    [SerializeField] private List<GameObject> coreNodes;
    [SerializeField] private List<GameObject> scrollerContents;
    [SerializeField] private GameObject basePanel;
    private NodeSave _nodeSaver;
    private List<GameObject> _customNodes = new List<GameObject>();


    /// <summary>
    /// Method <c>Start</c> sets up the scrollers and closes the menu.
    /// </summary>
    private void Start()
    {
        _nodeSaver = GetComponent<NodeSave>();
        _nodeSaver.SetCreatorManager(this);
        GetCustomNodes();
        SetupScrollers();
        CloseMenu();
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        NodeSave.NewSavedNode += UpdateCustomNodes;
        InteractiveNodeSpecsPanelManager.HideMenus += CloseMenu;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        NodeSave.NewSavedNode -= UpdateCustomNodes;
        InteractiveNodeSpecsPanelManager.HideMenus -= CloseMenu;
    }



    public GameObject BuildSavedNodePanel(GameObject node)
    {
        var node_func = node.GetComponentInChildren<BasicNode>();
        var new_panel = Instantiate(basePanel, scrollerContents[1].GetComponent<GridLayoutGroup>().transform);
        
        new_panel.GetComponentInChildren<Image>().sprite = 
            node_func.gameObject.GetComponent<SpriteRenderer>().sprite;
        new_panel.GetComponentInChildren<Image>().color = 
            node_func.gameObject.GetComponent<SpriteRenderer>().color;
        new_panel.GetComponentInChildren<TextMeshProUGUI>().text = node_func.name;
        new_panel.GetComponentInChildren<NodeCreatorBtn>().node = node;
        
        return new_panel;
    }
    
    /// <summary>
    /// Method <c>UpdateCustomNodes</c> gets all the custom made and saved nodes, then updates the scrollers.
    /// </summary>
    public void UpdateCustomNodes()
    {
        GetCustomNodes();
        SetupScrollers();
    }
    
    /// <summary>
    /// Method <c>GetCustomNodes</c> gets all the custom made and saved nodes.
    /// </summary>
    private void GetCustomNodes()
    {
        foreach (var node in _customNodes)
        {
            Destroy(node);
        }
        _customNodes = _nodeSaver.BuildAllSavedNodePanels();
    }
    
    /// <summary>
    /// Method <c>SetupScrollers</c> sets up the scrollers to be filled with the appropriate options.
    /// </summary>
    private void SetupScrollers()
    {
        for (var i = 0; i < scrollerContents.Count; i++)
        {
            var nodes = new[] {coreNodes, _customNodes}[i];
            var trans = scrollerContents[i].GetComponent<RectTransform>();
            var grid = scrollerContents[i].GetComponent<GridLayoutGroup>();
            
            // Adjusts the size to fit all objects
            trans.sizeDelta = new Vector2(grid.cellSize.x*nodes.Count + grid.spacing.x*nodes.Count, 
                trans.sizeDelta[1]);
            // Move scroller to the far left
            scrollerContents[i].transform.parent.GetComponentInParent<ScrollRect>().horizontalNormalizedPosition = 0;
        
            // Instantiate each node in the current scrollers' group
            if (i == 0)
            {
                foreach (var next_node in nodes.Select(Instantiate))
                {
                    next_node.transform.SetParent(grid.transform, false);
                }
            }
        }
    }
}