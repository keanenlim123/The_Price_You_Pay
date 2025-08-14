/// <summary>
/// BucketMop.cs  
/// This script manages the interaction between the player and the bucket-mop object.
/// When collected by the player, it plays a pickup sound (if assigned) and removes
/// the object from the scene. Designed for item collection mechanics in the game.
/// </summary>
/// <author> Keanen Lim Xi En </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10270417C </StudentID>
using UnityEngine;
public class BucketMop : MonoBehaviour
{
    /// <summary>
    /// The audio source that plays when the bucket-mop is picked up.
    /// </summary>
    public AudioSource pickupAudio;

    /// <summary>
    /// Called once before the first execution of Update after the MonoBehaviour is created.
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// Called once per frame to handle updates.
    /// </summary>
    void Update()
    {
        
    }

    /// <summary>
    /// Handles the collection of the bucket-mop by the player.
    /// Plays the pickup audio (if available and not already playing)
    /// and destroys the object afterwards.
    /// </summary>
    /// <param name="player">The PlayerBehaviour instance collecting the bucket-mop.</param>
    public void Collect(PlayerBehaviour player)
    {
        if (pickupAudio != null && !pickupAudio.isPlaying)
        {
            pickupAudio.Play();
        }
        Destroy(gameObject);
    }
}
