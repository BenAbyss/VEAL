using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ChromosomeCreationManager : MenuManager
{
    public static event Action<Chromosome, ChromosomeVariable> EnterVariable;
    public static event Action<Chromosome, string> SaveChromosome;
    
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
    public static string ParentChromosome;
    
    private Dictionary<int, ChromosomeVariable> _variables;
    private Dictionary<int, VariableDataPiece> _variableObjs;
    private Dictionary<int, FitnessDataPiece> _fitnessObjs;
    private Dictionary<int, string> _varNames;

    /// <summary>
    /// Method <c>Start</c> sets up the manager.
    /// </summary>
    public void Start()
    {
        _activePanel = ("Fitness", panels["Fitness"]);
        MutationCount = 1;
        CrossoverCount = 1;
        
        _variables = new Dictionary<int, ChromosomeVariable>();
        _variableObjs = new Dictionary<int, VariableDataPiece>();
        _fitnessObjs = new Dictionary<int, FitnessDataPiece>();
        _varNames = new Dictionary<int, string>();
        
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
        VariableDataPiece.TypeEntered += EnterVarType;
        ChromosomeSave.LoadChromosome += UpdateForChromosome;
        ChromosomeSave.ChromosomeLeft += s => {SaveChromosome?.Invoke(ExtractChromosome(), s); }; 
        ChromosomeSave.TopChromosomeReached += () => backBtn.SetActive(false);
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        LimitsSubmenuManager.VariableUpdated -= UpdateLimits;
        InputManager.CancelAction -= CloseMenu;
        VariableDataPiece.DeletedVariable -= RemoveVariable;
        VariableDataPiece.TypeEntered -= EnterVarType;
        ChromosomeSave.LoadChromosome -= UpdateForChromosome;
    }



    private void EnterVarType(int var_id)
    {
        backBtn.SetActive(true);
        EnterVariable?.Invoke(ExtractChromosome(), _variables[var_id]);
    }
    
    /// <summary>
    /// Method <c>UpdateForChromosome</c> updates all current input values to match that of the provided chromosome.
    /// <param name="chromosome">The chromosome to match inputs to.</param>
    /// </summary>
    private void UpdateForChromosome(Chromosome chromosome)
    {
        // if there's no current chromosome to base it off
        if (chromosome == null)
        {
            chromosome = new Chromosome();
        }
        
        // reset all appropriate elements
        _activePanel = ("Fitness", panels["Fitness"]);
        VariableDataPiece.VariablesCount = 0;
        FitnessDataPiece.VariablesCount = 0;
        _variables.Clear();
        foreach (Transform child in scrollers["Variable"].transform) if (child.name != "AddBtn") Destroy(child.gameObject);
        foreach (Transform child in scrollers["Fitness"].transform) Destroy(child.gameObject);
 

        var count = 1;

        if (chromosome.variables.Count == 0)
        {
            AddVarPrefab();
        }
        foreach (var variable in chromosome.variables)
        {
            AddVarPrefab();
            _variables[count] = variable;
            _variableObjs[count].UpdateVar(variable);
            _fitnessObjs[count].UpdateFitness(variable.chrName, variable.fitnessCalc, variable.fitnessWeight,
                variable.fitnessTargetVal);
            count++;
        }
        scrollers["Variable"].GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
        scrollers["Fitness"].GetComponentInParent<ScrollRect>().normalizedPosition = new Vector2(0, 1);
    }
    
    /// <summary>
    /// Method <c>ExtractChromosome</c> creates a chromosome from all the menu segments.
    /// <returns>The created chromosome from the menu</returns>
    /// </summary>
    private Chromosome ExtractChromosome()
    {
        var chromosome = new Chromosome {parentChromosome = ParentChromosome};
        var variables = _variables.Keys.Select(ExtractVariable).ToList();

        chromosome.variables = variables;
        return chromosome;
    }

    /// <summary>
    /// Method <c>ExtractVariable</c> extracts all the variables' data.
    /// <returns>The variables' data</returns>
    /// </summary>
    private ChromosomeVariable ExtractVariable(int var_id)
    {
        var variable = _variables[var_id];
        variable = _fitnessObjs[var_id].ApplyFitness(variable);
        variable.type = _variableObjs[var_id].GetVarType();
        return variable;
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
        AddPrefab(scrollers["Fitness"], dataPrefabs["Fitness"]);
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
        _varNames.Remove(var_id);
        _variables.Remove(var_id);
        Destroy(scrollers["Fitness"].transform.GetChild(var_id-1).gameObject);
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

        if (prefab_name != "Fitness")
        {
            addBtns[prefab_name].transform.SetSiblingIndex(child_count);
        }

        if (prefab_name == "Variable")
        {
            var var_data_piece = built_prefab.GetComponent<VariableDataPiece>();
            var_data_piece.SetManager(this);
            var_data_piece.Start();
            _variableObjs[var_data_piece.GetId()] = var_data_piece;
            _limitsManager.Invalids[var_data_piece.GetId()] = new List<string>();
            _limitsManager.Enumerators[var_data_piece.GetId()] = new List<(string, float)>();
        }
        else if (prefab_name == "Fitness")
        {
            var fitness_data_piece = built_prefab.GetComponent<FitnessDataPiece>();
            fitness_data_piece.Start();
            _fitnessObjs[fitness_data_piece.GetId()] = fitness_data_piece;
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
        // adjust appropriately for fitnesses lack of add button
        var btn_size = (scroller_name != "Fitness")
            ? ((RectTransform) addBtns[scroller_name].transform).rect.size : Vector2.zero;
        if (scroller_name == "Fitness") content_amnt++;
        
        // Adjusts the size to fit all objects
        try
        {
            grid = (VerticalLayoutGroup) grid;
            trans.sizeDelta = new Vector2(trans.sizeDelta[0],
                ((RectTransform) dataPrefabs[scroller_name].transform).rect.height * content_amnt
                + grid.spacing * content_amnt + grid.padding.top * 2 + btn_size.y);
        }
        catch (InvalidCastException)
        {
            grid = (HorizontalLayoutGroup) grid;
            trans.sizeDelta = new Vector2(((RectTransform) dataPrefabs[scroller_name].transform).rect.width 
                                          * content_amnt + grid.spacing * content_amnt + grid.padding.left * 2 +
                                          btn_size.x, trans.sizeDelta[1]);
        }

        // Move scroller to the top
        scroller.transform.parent.GetComponentInParent<ScrollRect>().verticalNormalizedPosition = 0;
    }



    /// <summary>
    /// Method <c>NewVarName</c> ensures the name is unique, then updates it for the variable and linked fitness.
    /// <param name="new_name">The newly entered name.</param>
    /// <param name="var_id">The ID of the variable having it's name changed.</param>
    /// </summary>
    public void NewVarName(string new_name, int var_id)
    {
        if (new_name == "") return;
        
        var adjusted_val = 0;
        if (!_varNames.ContainsKey(var_id)) _varNames[var_id] = "";
        
        while (_varNames.ContainsValue(new_name) && _varNames[var_id] != new_name)
        {
            if (adjusted_val > 0) new_name = new_name.Substring(0, new_name.Length - 2);
            adjusted_val++;
            new_name += "_" + adjusted_val;
        }

        _varNames[var_id] = new_name;
        _variableObjs[var_id].SetName(new_name);
        _variables[var_id].chrName = new_name;
        _fitnessObjs[var_id].SetVarName(new_name);
    }
    
    /// <summary>
    /// Method <c>UpdateLimits</c> updates the limits of a specific variable.
    /// <param name="limits">The new variable limits.</param>
    /// <param name="var_id">The id of the variable to update.</param>
    /// </summary>
    private void UpdateLimits(ChromosomeLimits limits, int var_id)
    {
        _variables[var_id].limits = limits;
        _variableObjs[var_id].UpdateText(limits);
    }
}
