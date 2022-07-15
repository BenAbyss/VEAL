using System;
using System.Collections.Generic;
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
    [SerializeField] private GameObject backBtn;
    [SerializeField] private GameObject limitsMenu;
    
    public static int MutationCount;
    public static int CrossoverCount;
    private LimitsSubmenuManager _limitsManager;
    private (string, GameObject) _activePanel;
    private Dictionary<int, ChromosomeVariable> _variables;

    /// <summary>
    /// Method <c>Start</c> sets up the manager.
    /// </summary>
    public void Start()
    {
        _activePanel = ("Fitness", panels["Fitness"]);
        MutationCount = 1;
        CrossoverCount = 1;
        _variables = new Dictionary<int, ChromosomeVariable>();
        _limitsManager = limitsMenu.GetComponent<LimitsSubmenuManager>();
        _limitsManager.CloseMenuUnsaved();
        AddVarPrefab();
    }
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        LimitsSubmenuManager.VariableUpdated += UpdateLimits;
        InputManager.CancelAction += CloseMenu;
        VariableDataPiece.DeletedVariable += RemoveVariable;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        LimitsSubmenuManager.VariableUpdated -= UpdateLimits;
        InputManager.CancelAction -= CloseMenu;
        VariableDataPiece.DeletedVariable -= RemoveVariable;
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
    /// Method <c>OpenLimitsMenu</c> enables and sets up the limits menu.
    /// <param name="var_id">The ID of the variable calling this.</param>
    /// <param name="type">The type of the variable calling this.</param>
    /// </summary>
    public void OpenLimitsMenu(int var_id, VarType type)
    {
        _limitsManager.ChangeActivity(true);
        _limitsManager.SetupScene(_variables[var_id].limits, var_id, type);
    }



    /// <summary>
    /// Method <c>AddPrefab</c> extends the scroller size and creates the new prefab for the Variable panel.
    /// This exists for the purpose of button calls.
    /// </summary>
    public void AddVarPrefab()
    {
        _variables[VariableDataPiece.VariablesCount+1] = new ChromosomeVariable(VarType.Integer);
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

    private void RemoveVariable(int var_id)
    {
        _variables.Remove(var_id);
        var scroller = scrollers["Variable"];
        AdjustScroller(scroller, "Variable", scroller.GetComponent<VerticalLayoutGroup>(), 
            scroller.GetComponent<RectTransform>(), scroller.transform.childCount-1);
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

        if (prefab_name == "Variable")
        {
            var var_data_piece = built_prefab.GetComponent<VariableDataPiece>();
            var_data_piece.SetManager(this);
            var_data_piece.Start();
            _limitsManager.Invalids[var_data_piece.GetId()] = new List<string>();
        }
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
    
    /// <summary>
    /// Method <c>UpdateLimits</c> updates the limits of a specific variable.
    /// <param name="limits">The new variable limits.</param>
    /// <param name="var_id">The id of the variable to update.</param>
    /// </summary>
    private void UpdateLimits(ChromosomeLimits limits, int var_id)
    {
        _variables[var_id].limits = limits;
    }
}
