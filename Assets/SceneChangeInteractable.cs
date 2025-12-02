using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChangeInteractable : MonoBehaviour
{
    [Header("Scene Configuration")]
    [Tooltip("The *exact* name of the scene you want to load.")]
    public string targetSceneName;

    [Tooltip("The *exact* name of the GameObject in the new scene to spawn at.")]
    public string targetSpawnPointName;

    [Tooltip("(Optional) The name of the scene to UNLOAD. Leave empty if you're loading a hub, etc.")]
    public string sceneToUnload;

    [Header("Physics")]
    [Tooltip("The gravity for the target scene. (0, 0, 0) for space.")]
    public Vector3 sceneGravity = new Vector3(0, -9.81f, 0);

    // This static variable holds our player's transform.
    // 'static' means it's shared across all instances of this script,
    // so we only need to find the player one time.
    private static Transform playerTransform;

    /// <summary>
    /// This is the public function you must call from your XR Grab Interactable event.
    /// </summary>
    public void LoadTargetScene()
    {
        // Check for missing scene or spawn names in the Inspector
        if (string.IsNullOrEmpty(targetSceneName) || string.IsNullOrEmpty(targetSpawnPointName))
        {
            Debug.LogWarning("Scene Name or Spawn Point Name is not set on " + gameObject.name, this);
            return;
        }

        // --- 1. Find the player by tag (if we haven't already) ---
        if (playerTransform == null)
        {
            // This searches your *entire* project hierarchy for the object tagged "Player".
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                playerTransform = playerObj.transform;
            }
            else
            {
                // This is a critical error. Stop here.
                Debug.LogError("Could not find the player! Make sure your XR Origin is tagged 'Player'.", this);
                return;
            }
        }

        // --- 2. Start the coroutine to handle the scene change ---
        // We use a Coroutine because loading/unloading scenes takes time.
        StartCoroutine(LoadSceneAndTeleport());
    }

    /// <summary>
    /// This coroutine handles the asynchronous loading and unloading of scenes,
    /// then teleports the player.
    /// </summary>
    private IEnumerator LoadSceneAndTeleport()
    {
        // --- 1. UNLOAD OLD SCENE (if specified) ---
        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            Scene oldScene = SceneManager.GetSceneByName(sceneToUnload);
            foreach (GameObject rootObj in oldScene.GetRootGameObjects())
            {
                foreach (AudioSource audio in rootObj.GetComponentsInChildren<AudioSource>())
                {
                    audio.Stop();
                }
            }
            if (oldScene.isLoaded)
            {
                Debug.Log("Unloading scene: " + sceneToUnload);
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }
        }

        // --- 2. LOAD NEW SCENE ---
        Debug.Log("Loading scene: " + targetSceneName);
        // We load 'Additive' so it doesn't destroy our persistent player scene.
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);

        // Wait here until the scene is fully loaded
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // --- 3. SET GRAVITY ---
        // This is the global physics change.
        Debug.Log("Setting gravity to: " + sceneGravity);
        Physics.gravity = sceneGravity;

        // --- 4. FIND SPAWN POINT ---
        // The scene is now loaded, so we can safely find the spawn point by its name.
        GameObject spawnPoint = GameObject.Find(targetSpawnPointName);
        Transform spawnTransform = null;

        if (spawnPoint != null)
        {
            spawnTransform = spawnPoint.transform;
        }
        else
        {
            // We loaded the scene, but couldn't find the spawn point.
            // This is a common error caused by a typo in the name.
            Debug.LogError($"Could not find spawn point '{targetSpawnPointName}' in scene '{targetSceneName}'! Check spelling.", this);
        }

        // --- 5. SET ACTIVE SCENE ---
        // This is important for lighting and skyboxes to work correctly.
        Scene newScene = SceneManager.GetSceneByName(targetSceneName);
        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
        }
        else
        {
            Debug.LogError("Failed to load or find scene: " + targetSceneName, this);
            yield break; // Exit coroutine
        }

        // --- 6. TELEPORT PLAYER ---
        // Finally, move the player to the spawn point
        if (playerTransform != null && spawnTransform != null)
        {
            Debug.Log("Teleporting player to " + spawnTransform.name);
            // We move the entire Rig (playerTransform), not just the camera.
            playerTransform.position = spawnTransform.position;
            playerTransform.rotation = spawnTransform.rotation;
        }
        else
        {
            Debug.LogError("Teleport failed. Player or Spawn Point was not found.", this);
        }
    }
}