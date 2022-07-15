using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private float transitionTime;
    protected bool IsActive;
    protected GameObject MenuObject;
    protected CanvasGroup Canvas;

    /// <summary>
    /// Method <c>Awake</c> sets up object variables.
    /// </summary>
    public void Awake()
    {
        MenuObject = transform.gameObject;
        Canvas = MenuObject.GetComponent<CanvasGroup>();
    }
    
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
    /// Method <c>LoadScene</c> asynchronously loads a new scene.
    /// <param name="scene">The name of the scene to load.</param>
    /// </summary>
    public IEnumerator LoadScene(string scene)
    {
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation async_operation = SceneManager.LoadSceneAsync(scene);
        // returns control back to the caller until it's complete
        while (!async_operation.isDone)
        {
            yield return null;
        }
    }

    /// <summary>
    /// Method <c>ToggleActive</c> toggles the active state of the menu.
    /// </summary>
    public void ToggleActive()
    {
        ChangeActivity(!IsActive);
    }

    /// <summary>
    /// Method <c>ChangeActivity</c> changes the activity of the menu appropriately.
    /// <param name="enable">Whether to enable the activity or not.</param>
    /// </summary>
    public void ChangeActivity(bool enable)
    {
        IsActive = enable;
        Canvas.alpha = enable ? 1 : 0;
        Canvas.interactable = enable;
        Canvas.blocksRaycasts = enable;
    }
    
    /// <summary>
    /// Method <c>CloseMenu</c> closes the menu.
    /// </summary>
    public virtual void CloseMenu()
    {
        ChangeActivity(false);
    }
}