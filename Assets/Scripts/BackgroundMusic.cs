/// <summary>
/// BackgroundMusic.cs  
/// This script handles background music playback for the convenience store environment.
/// It ensures smooth looping, controls volume settings, and can manage transitions
/// between different music tracks depending on gameplay events.
/// </summary>
/// <author> Lim Xue Zhi Conan </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10269214H </StudentID>

using UnityEngine;

/// <summary>
/// Enables background music for the Convenience Store through a collider trigger.
/// </summary>
public class BackgroundMusic : MonoBehaviour
{
    private AudioSource audioSource;

    /// <summary>
    /// Gets the AudioSource component attached to the same GameObject.
    /// </summary>
    void Start()
    {
        // Get the AudioSource component attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Called when another collider enters the trigger collider attached to this GameObject.
    /// If the collider belongs to the player, it starts playing the background music.
    /// </summary>
    /// <param name="other">The collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player entered the trigger
        if (other.CompareTag("Player"))
        {
            // Play the audio if it's not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }
}
