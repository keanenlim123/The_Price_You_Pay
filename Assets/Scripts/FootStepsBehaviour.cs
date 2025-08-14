/// <summary>
/// FootStepsBehaviour.cs  
/// This script handles the behavior of muddy footprints in the game.  
/// It tracks the total number of footprints the player has cleaned, as well as the total spawned in the scene.  
/// When the player cleans a footprint, the script increments the cleaned counter, logs relevant debug messages,  
/// and removes the footprint from the scene. This is tied to the game’s objective system,  
/// particularly for objectives requiring the cleanup of muddy footprints.  
/// </summary>
/// <author>Chia Jia Cong Justin</author>
/// <date>10/8/2025</date>
/// <StudentID>S10266690C</StudentID>

using UnityEngine;

/// <summary>
/// Handles the behavior of muddy footprints in the game world, specifically tracking
/// how many footprints have been cleaned and destroyed by the player.
/// </summary>
public class FootStepsBehaviour : MonoBehaviour
{
    /// <summary>
    /// Tracks the total number of footprints that have been cleaned by the player.
    /// This value is shared across all instances of <see cref="FootStepsBehaviour"/>.
    /// It increases when the <see cref="Clean"/> method is called.
    /// </summary>
    [SerializeField]
    public static int footstepamount = 0;

    /// <summary>
    /// Tracks the total number of muddy footprints that have been spawned in the game.
    /// This is useful for monitoring or limiting the total number of footprints active in the scene.
    /// </summary>
    public static int totalSpawned = 0;

    /// <summary>
    /// Cleans this specific footprint instance.  
    /// When called, it increments <see cref="footstepamount"/> to reflect
    /// the player's progress, logs relevant information for debugging,
    /// and removes the footprint object from the scene.
    /// </summary>
    public void Clean()
    {
        footstepamount++;
        Debug.Log("CLEAN");
        Debug.Log(footstepamount);
        Destroy(gameObject);
    }
}