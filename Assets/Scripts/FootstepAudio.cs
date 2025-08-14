/// <summary>
/// FootstepAudio.cs  
/// This script manages the player's footstep sounds, ensuring they play at the correct timing
/// when the player is walking or running. It can be configured to handle different
/// surfaces and adjust sound volume or pitch for realism.
/// </summary>
/// <author> Lim Xue Zhi Conan </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10269214H </StudentID>


using UnityEngine;

/// <summary>
/// Enables the user to import multiple audio sources for footstep noises,
/// and calculates the movement and timing required for a footstep
/// to be played as a sound.
/// </summary>
public class FootstepAudio : MonoBehaviour
{
    /// <summary>
    /// Array of footstep sound clips. Multiple variations can be added here.
    /// </summary>
    public AudioClip[] footstepClips;

    /// <summary>
    /// The time interval (in seconds) between each footstep sound.
    /// </summary>
    public float stepInterval = 0.5f;

    /// <summary>
    /// The minimum movement speed required for footsteps to be triggered.
    /// </summary>
    public float moveThreshold = 0.1f;

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer;

    /// <summary>
    /// Initializes the AudioSource and CharacterController components.
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Updates the step timer based on player movement and plays footsteps if conditions are met.
    /// </summary>
    void Update()
    {
        if (controller.isGrounded && controller.velocity.magnitude > moveThreshold)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f; // Reset timer when not moving
        }
    }

    /// <summary>
    /// Plays a random footstep sound from the available clips.
    /// </summary>
    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[index]);
        }
    }
}
