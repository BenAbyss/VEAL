using System.Collections.Generic;
using UnityEngine;

public class NodeCreatorMenuManager : MenuManager
{
    [SerializeField] private List<GameObject> coreNodes;
    private List<GameObject> _customNodes;
    
    /// <summary>
    /// Method <c>OnEnable</c> sets reactionary method calls to invoked events.
    /// </summary>
    protected void OnEnable()
    {
        InputManager.CancelAction += CloseMenu;
    }

    /// <summary>
    /// Method <c>OnDisable</c> disables reactionary method calls to invoked events.
    /// </summary>
    protected void OnDisable()
    {
        InputManager.CancelAction -= CloseMenu;
    }
    
    /// <summary>
    /// Method <c>CloseMenu</c> closes the menu.
    /// </summary>
    private void CloseMenu(){}
}