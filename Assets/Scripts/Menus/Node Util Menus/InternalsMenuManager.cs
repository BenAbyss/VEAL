using System;
using TMPro;
using UnityEngine;

public class InternalsMenuManager : MenuManager
{
    public static event Action PreviousScene;
    public static event Action ReturnToChromosome;
    [SerializeField] private TextMeshProUGUI text;

    public void Start()
    {
        ChangeActivity(false);
        if (ItemCreating != "Nodes")
        {
            UpdateText(ItemCreating, true);
        }
    }

    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected new void OnEnable()
    {
        SceneSave.SceneChanged += UpdateText;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected new void OnDisable()
    {
        SceneSave.SceneChanged -= UpdateText;
    }
    
    
    
    /// <summary>
    /// Method <c>UpdateText</c> updates the title text, and makes it visible if it's an internal scene still.
    /// <param name="new_text">The new title to show.</param>
    /// <param name="is_internal">Whether the scene is internal.</param>
    /// </summary>
    private void UpdateText(string new_text, bool is_internal)
    {
        text.text = new_text;
        if (is_internal)
        {
            ChangeActivity(true);
        }
    }

    /// <summary>
    /// Method <c>Return</c> returns to the previous scene.
    /// </summary>
    public void Return()
    {
        ChangeActivity(false);
        if (ItemCreating != "Nodes")
        {
            ItemCreating = "Chromosome";
            ReturnToChromosome?.Invoke();
            StartCoroutine(LoadScene("Chromosome Creation"));
        }
        else
        {
            PreviousScene?.Invoke();
        }
    }
}