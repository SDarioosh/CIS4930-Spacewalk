using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections; // Required for Coroutines (IEnumerator)

public class SceneChangeInteractable : MonoBehaviour
{
    [Tooltip("The *exact* name of the scene you want to load. Must be in Build Settings!")]
    public string targetSceneName;

    public void LoadTargetScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            // Start the coroutine to load the scene and set it active
            StartCoroutine(LoadSceneAdditivelyAndSetActive());
        }
        else
        {
            Debug.LogWarning("Target Scene Name is not set on " + gameObject.name, this);
        }
    }

    /// <summary>
    /// Loads the target scene additively and asynchronously, then sets it as the active scene.
    /// </summary>
    private IEnumerator LoadSceneAdditivelyAndSetActive()
    {
        // 1. Start loading the new scene additively and asynchronously
        // This returns an AsyncOperation object that we can use to track progress
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        // 2. Wait for the scene to finish loading
        // We use 'while (!loadOperation.isDone)' to wait until the loading is complete
        while (!loadOperation.isDone)
        {
            // yield return null waits until the next frame, preventing the game from freezing
            yield return null;
        }

        // 3. Once loaded, get the new scene by its name
        Scene newScene = SceneManager.GetSceneByName(targetSceneName);

        // 4. Set the newly loaded scene as the active scene
        if (newScene.IsValid()) // Check if the scene was loaded successfully
        {
            SceneManager.SetActiveScene(newScene);
        }
        else
        {
            Debug.LogError("Failed to load or find scene: " + targetSceneName, this);
        }
    }
}