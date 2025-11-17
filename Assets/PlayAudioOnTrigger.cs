using UnityEngine;

public class PlayAudioOnTrigger : MonoBehaviour
{
    [Tooltip("Drag the AudioSource you want to play here.")]
    public AudioSource audioToPlay;

    [Tooltip("Should this sound only play once?")]
    public bool playOnce = true;

    private bool hasPlayed = false;

    // This function is called by Unity when another collider enters this trigger.
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is tagged "Player"
        if (other.CompareTag("Player"))
        {
            // If we only want to play it once, check if it has already played
            if (playOnce && !hasPlayed)
            {
                // It's the player and it hasn't played!
                hasPlayed = true; // Mark it as played
                audioToPlay.Play(); // Play the sound
            }
            else if (!playOnce)
            {
                // If we don't care about playing it once, just play it.
                audioToPlay.Play();
            }
        }
    }
}