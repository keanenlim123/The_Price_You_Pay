/// <summary>
/// StolenItemBehaviour.cs  
/// This script manages the behavior of items that can be stolen by the player.  
/// When the player interacts with the stolen item, it marks the player as having stolen the item,  
/// plays a pickup sound (if available), and removes the item from the scene.  
/// This mechanic ties into the game's objectives or consequences system, as stealing may trigger  
/// AI responses, objectives updates, or game progression changes.  
/// </summary>
/// <author>Chia Jia Cong Justin</author>
/// <date>10/8/2025</date>
/// <StudentID>S10266690C</StudentID>
using UnityEngine;

public class StolenItemBehaviour : MonoBehaviour
{
    /// <summary>
    /// The audio source used to play a sound when the item is stolen.  
    /// Optional — only plays if assigned and not already playing.
    /// </summary>
    public AudioSource pickupAudio;

    /// <summary>
    /// Handles the logic for when a player steals this item.  
    /// Marks the player as having stolen an item, removes the item from the scene,  
    /// logs the event for debugging, and plays an audio cue (if available).
    /// </summary>
    /// <param name="player">The PlayerBehaviour instance representing the player stealing the item.</param>
    public void Steal(PlayerBehaviour player)
    {
        // Mark the player as having stolen an item
        player.hasStolenItem = true;

        // Destroy the stolen item from the scene
        Destroy(gameObject);

        Debug.Log("Item stolen!");

        // Play pickup sound if assigned and not already playing
        if (pickupAudio != null && !pickupAudio.isPlaying)
        {
            pickupAudio.Play();
        }
    }
}
