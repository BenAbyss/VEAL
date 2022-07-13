using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChromosomeCreationManager : MenuManager
{
    [SerializeField] private SerializableStringGameObjDict panels;
    [SerializeField] private SerializableStringGameObjDict addBtns;
    [SerializeField] private SerializableStringGameObjDict dataPrefabs;
    [SerializeField] private SerializableStringGameObjDict scrollers;
    [SerializeField] private string[] panelOrder;
    
    public static int MutationCount;
    public static int CrossoverCount;
    private GameObject _backBtn;
    private (string, GameObject) _activePanel;

    /// <summary>
    /// Method <c>Start</c> sets up the manager.
    /// </summary>
    public void Start()
    {
        _activePanel = ("Fitness", panels["Fitness"]);
        MutationCount = 1;
        CrossoverCount = 1;
        _backBtn = GameObject.Find("InternalsBackBtn");
        _backBtn.SetActive(false);
    }
    
    
    
    /// <summary>
    /// Method <c>MovePanelsLeft</c> swaps the top panel to the next panel leftwards.
    /// </summary>
    public void MovePanelsLeft()
    {
        SwapTopPanels(-1);
    }
    
    /// <summary>
    /// Method <c>MovePanelsRight</c> swaps the top panel to the next panel rightwards.
    /// </summary>
    public void MovePanelsRight()
    {
        SwapTopPanels(1);
    }

    /// <summary>
    /// Method <c>AddPrefab</c> extends the scroller size and creates the new prefab for the Variable panel.
    /// This exists for the purpose of button calls.
    /// </summary>
    public void AddVarPrefab()
    {
        AddPrefab(scrollers["Variable"], dataPrefabs["Variable"]);
    }

    /// <summary>
    /// Method <c>AddPrefab</c> extends the scroller size and creates the new prefab for the Mutation panel.
    /// This exists for the purpose of button calls.
    /// </summary>
    public void AddMutationPrefab()
    {
        MutationCount++;
        AddPrefab(scrollers["Mutation"], dataPrefabs["Mutation"]);
    }

    /// <summary>
    /// Method <c>AddPrefab</c> extends the scroller size and creates the new prefab for the Crossover panel.
    /// This exists for the purpose of button calls.
    /// </summary>
    public void AddCrossoverPrefab()
    {
        CrossoverCount++;
        AddPrefab(scrollers["Crossover"], dataPrefabs["Crossover"]);
    }



    /// <summary>
    /// Method <c>AddPrefab</c> extends the scroller size and creates the new prefab.
    /// <param name="scroller">The scroller to add the prefab to.</param>
    /// <param name="prefab">The prefab to create.</param>
    /// </summary>
    private void AddPrefab(GameObject scroller, GameObject prefab)
    {
        var grid = scroller.GetComponent<HorizontalOrVerticalLayoutGroup>();
        var child_count = scroller.transform.childCount;
        var prefab_name = Regex.Split(prefab.name, @"(?<!^)(?=[A-Z])")[0];
        // this doesn't require a +1 thanks to the add button
        AdjustScroller(scroller, prefab_name, grid, scroller.GetComponent<RectTransform>(), child_count); 
        // instantiate the prefab
        CreatePrefab(prefab, prefab_name, grid, child_count);
    }
    
    /// <summary>
    /// Method <c>CreatePrefab</c> creates and sets up a path name prefab, placing it in the scroller.
    /// <param name="prefab">The prefab to create.</param>
    /// <param name="prefab_name">The name of the prefab - exclusively the evolutionary action word.</param>
    /// <param name="grid">The grid to create it to.</param>
    /// <param name="child_count">The amount of children the grid have.</param>
    /// </summary>
    private void CreatePrefab(GameObject prefab, string prefab_name, 
        HorizontalOrVerticalLayoutGroup grid, int child_count)
    {
        var built_prefab = Instantiate(prefab, grid.transform, false);
        addBtns[prefab_name].transform.SetSiblingIndex(child_count);
    }
    
    /// <summary>
    /// Method <c>SwapTopPanels</c> swaps the top panel to the appropriate next panel.
    /// <param name="dir">The direction of the panels to head, -1 being left and +1 being right.</param>
    /// </summary>
    private void SwapTopPanels(int dir)
    {
        var new_index = Array.IndexOf(panelOrder, _activePanel.Item1) + dir;
        if (new_index >= panelOrder.Length)
        {
            new_index = 0;
        } else if (new_index < 0)
        {
            new_index = panelOrder.Length - 1;
        }
        _activePanel.Item2.SetActive(false);
        _activePanel = (panelOrder[new_index], panels[panelOrder[new_index]]);
        _activePanel.Item2.SetActive(true);
    }

    /// <summary>
    /// Method <c>AdjustScroller</c> adjusts the scroller to the appropriate size.
    /// <param name="scroller">The object of the scroller to adjust.</param>
    /// <param name="grid">The layout grid of the scroller.</param>
    /// <param name="trans">The transform of the scroller.</param>
    /// <param name="content_amnt">The amount of content within the scroller.</param>
    /// </summary>
    private void AdjustScroller(GameObject scroller, string scroller_name, HorizontalOrVerticalLayoutGroup grid, 
        RectTransform trans, int content_amnt)
    {
        // Adjusts the size to fit all objects
        try
        {
            grid = (VerticalLayoutGroup) grid;
            trans.sizeDelta = new Vector2(trans.sizeDelta[0],
                ((RectTransform) dataPrefabs[scroller_name].transform).rect.height * content_amnt
                + grid.spacing * content_amnt + grid.padding.top * 2 +
                ((RectTransform) addBtns[scroller_name].transform).rect.height);
        }
        catch (InvalidCastException)
        {
            grid = (HorizontalLayoutGroup) grid;
            trans.sizeDelta = new Vector2(((RectTransform) dataPrefabs[scroller_name].transform).rect.width 
                                          * content_amnt + grid.spacing * content_amnt + grid.padding.left * 2 +
                                          ((RectTransform) addBtns[scroller_name].transform).rect.width, 
                                         trans.sizeDelta[1]);
        }

        // Move scroller to the top
        scroller.transform.parent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}
