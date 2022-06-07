using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NodeCreatorMenuManager : MenuManager
{
    [SerializeField] private List<GameObject> coreNodes;
    [SerializeField] private List<GameObject> scrollerContents;
    private List<GameObject> _customNodes = new List<GameObject>();


    /// <summary>
    /// Method <c>Start</c> sets up the scrollers and closes the menu.
    /// </summary>
    private void Start()
    {
        SetupScrollers();
        CloseMenu();
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
            foreach (var next_node in nodes.Select(Instantiate))
            {
                next_node.transform.SetParent(grid.transform, false);
            }
        }
    }
}