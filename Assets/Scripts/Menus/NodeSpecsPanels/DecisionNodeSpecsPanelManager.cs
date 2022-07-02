using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DecisionNodeSpecsPanelManager : NodeSpecsPanelManager
{
    [SerializeField] private GameObject namesScroller;
    [SerializeField] private TextMeshProUGUI pathsToTake;
    [SerializeField] private GameObject pathNamePrefab;
    [SerializeField] private GameObject addBtn;
    
    private DecisionNode _node;
    private List<DecisionNodePathNameSubsection> _pathSections;
    private VerticalLayoutGroup _grid;
    private RectTransform _trans;

    /// <summary>
    /// Method <c>Start</c> fetches necessary components.
    /// </summary>
    public void Start()
    {
        _trans = namesScroller.GetComponent<RectTransform>();
        _grid = namesScroller.GetComponent<VerticalLayoutGroup>();
        _pathSections = new List<DecisionNodePathNameSubsection>();
    }

    /// <summary>
    /// Method <c>ChangeNode</c> sets the selected node and changes the visual values to the nodes'.
    /// <param name="new_node">The new node to be selected.</param>
    /// </summary>
    public override void ChangeNode(InteractiveNode new_node)
    {
        _node = (DecisionNode) new_node;
        pathsToTake.text = _node.GetPathsTaken().ToString();
        RevertPathNames();
    }

    /// <summary>
    /// Method <c>SetPathName</c> sets the path name for the appropriately positioned path.
    /// <param name="new_name">The new path name.</param>
    /// <param name="pos">The path position.</param>
    /// </summary>
    public void SetPathName(string new_name, int pos)
    {
        _node.SetName(new_name, pos);
    }

    /// <summary>
    /// Method <c>SetPathsToTake</c> sets the selected nodes' number of paths taken.
    /// </summary>
    public void SetPathsToTake()
    {
        StartCoroutine(TimedSetPathsToTake());
    }

    /// <summary>
    /// Method <c>TimedSetPathsToTake</c> sets the selected nodes' number of paths taken after a brief pause.
    /// </summary>
    IEnumerator TimedSetPathsToTake()
    {
        _node.SetPathsTaken(Convert.ToInt32(pathsToTake.text));
        yield return new WaitForSecondsRealtime(0.1f);
    }

    /// <summary>
    /// Method <c>AddPathPrefab</c> extends the scroller size and creates the new prefab.
    /// </summary>
    public void AddPathPrefab()
    {
        // this doesn't require a +1 thanks to the add button
        AdjustScroller(namesScroller.transform.childCount); 
        // instantiate the prefab
        CreatePrefab();
    }

    /// <summary>
    /// Method <c>RemovePath</c> removes a path and decrements those after.
    /// </summary>
    public void RemovePath(int pos)
    {
        for (int i = pos; i < _pathSections.Count; i++)
        {
            _pathSections[i - 1] = _pathSections[i];
            _pathSections[i - 1].SetPos(i);
        }
    }

    

    /// <summary>
    /// Method <c>CreatePrefab</c> creates and sets up a path name prefab, placing it in the scroller.
    /// <param name="path_name">The name of the path, if already known.</param>
    /// </summary>
    private void CreatePrefab(string path_name = "")
    {
        var prefab = Instantiate(pathNamePrefab, _grid.transform, false);
        var prefab_code = prefab.GetComponent<DecisionNodePathNameSubsection>();
        _pathSections.Add(prefab_code);
        prefab_code.SetManager(this);
        prefab_code.SetPos(_pathSections.Count);
        if (path_name != "")
        {
            prefab_code.SetName(path_name);
        }
        addBtn.transform.SetSiblingIndex(namesScroller.transform.childCount);
    }
    
    /// <summary>
    /// Method <c>AdjustScroller</c> adjusts the scroller to the appropriate size.
    /// </summary>
    private void AdjustScroller(int content_amnt)
    {
        if (_trans == null)
        {
            Start();
        }
        
        // Adjusts the size to fit all objects
        _trans.sizeDelta = new Vector2(_trans.sizeDelta[0], 
            ((RectTransform)pathNamePrefab.transform).rect.height*content_amnt 
            + _grid.spacing*content_amnt + _grid.padding.top*2 + ((RectTransform)addBtn.transform).rect.height);
        // Move scroller to the top
        namesScroller.transform.parent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
    }
    
    /// <summary>
    /// Method <c>RevertPathNames</c> deletes/creates the correct amount of path names,
    /// and inserts the names in their positions.
    /// </summary>
    private void RevertPathNames()
    {
        var names = _node.GetNames();
        
        foreach (Transform child in namesScroller.transform)
        {
            if (child.name != "AddBtn")
            {
                Destroy(child.gameObject);
            }
        }
        _pathSections = new List<DecisionNodePathNameSubsection>();
        
        AdjustScroller(names.Count+1);
        foreach (var next_name in names)
        {
            CreatePrefab(next_name);
        }
    }
}