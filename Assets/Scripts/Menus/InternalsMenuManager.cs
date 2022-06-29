using System;
using TMPro;
using UnityEngine;

public class InternalsMenuManager : MenuManager
{
    public static event Action PreviousScene;
    [SerializeField] private TextMeshProUGUI text;

    public void Start()
    {
        ChangeActivity(false);
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
    
    
    
    private void UpdateText(string new_text)
    {
        text.text = new_text;
        ChangeActivity(true);
    }

    public void Return()
    {
        PreviousScene?.Invoke();
        ChangeActivity(false);
    }
}