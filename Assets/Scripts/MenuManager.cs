using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private float transitionTime;

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
}