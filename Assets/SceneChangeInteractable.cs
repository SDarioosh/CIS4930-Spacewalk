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

    [Tooltip("(Optional) The name of the scene to UNLOAD. Leave empty for your main hub (like the station).")]
    public string sceneToUnload;

    [Header("Physics")]
    [Tooltip("The gravity for the target scene. (0, 0, 0) for space.")]
    public Vector3 sceneGravity = new Vector3(0, -9.81f, 0);

    // Private variable to hold our player.
    // 'static' means it will be shared by ALL teleporters,
    // so we only have to find it once.
    private static Transform playerTransform;

    public void LoadTargetScene()
    {
        // Check for missing scene or spawn names
        if (string.IsNullOrEmpty(targetSceneName) || string.IsNullOrEmpty(targetSpawnPointName))
        {
            Debug.LogWarning("Scene Name or Spawn Point Name is not set on " + gameObject.name, this);
            return;
        }

        // 1. Find the player by tag (if we haven't already)
        if (playerTransform == null)
        {
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

        // 2. Start the coroutine to handle the scene change and teleport
        StartCoroutine(LoadSceneAndTeleport());
    }

    private IEnumerator LoadSceneAndTeleport()
    {
        // --- 1. UNLOAD OLD SCENE (if specified) ---
        if (!string.IsNullOrEmpty(sceneToUnload))
        {
            Scene oldScene = SceneManager.GetSceneByName(sceneToUnload);
            if (oldScene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(sceneToUnload);
            }
        }

        // --- 2. LOAD NEW SCENE ---
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // --- 3. SET GRAVITY ---
        Physics.gravity = sceneGravity;

        // --- 4. FIND SPAWN POINT ---
        // The scene is now loaded, so we can find the spawn point by its name.
        GameObject spawnPoint = GameObject.Find(targetSpawnPointName);
        Transform spawnTransform = null;

        if (spawnPoint != null)
        {
            spawnTransform = spawnPoint.transform;
        }
        else
        {
            // We loaded the scene, but couldn't find the spawn point.
            Debug.LogError($"Could not find spawn point '{targetSpawnPointName}' in scene '{targetSceneName}'!", this);
        }

        // --- 5. SET ACTIVE SCENE ---
        // This is important for lighting and skyboxes
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
            // For XR Origin, it's safer to move the Rig, not the Camera
            // We assume 'playerTransform' is the XR Origin (the parent)
            playerTransform.position = spawnTransform.position;
            playerTransform.rotation = spawnTransform.rotation;
        }
        else
        {
            Debug.LogError("Teleport failed. Player or Spawn Point could not be found.", this);
        }
    }
}