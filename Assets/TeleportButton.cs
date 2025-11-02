using UnityEngine;

// You might need this namespace if you are using the XROrigin component
// using Unity.XR.CoreUtils; 

public class TeleporterButton : MonoBehaviour
{
    [Tooltip("The XR Origin (your player) to teleport.")]
    public GameObject xrOrigin;

    [Tooltip("The exact coordinates to teleport the player to.")]
    public Vector3 targetCoordinates;

    // This is the public function we will call from the button event
    public void TeleportPlayer()
    {
        if (xrOrigin != null)
        {
            // Instantly move the player's origin to the target coordinates
            xrOrigin.transform.position = targetCoordinates;
        }
        else
        {
            Debug.LogWarning("XR Origin is not assigned in the TeleporterButton script!");
        }
    }
}