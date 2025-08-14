/// <summary>
/// SlidingDoorBehaviour.cs  
/// This script controls a pair of sliding doors in the game, used for the store entrance.  
/// The doors slide open when the player enters a trigger zone and close when the player exits.  
/// It uses Lerp to smoothly transition between open and closed positions for both left and right door panels.  
/// Audio feedback is provided for both opening and closing actions, ensuring each sound is only played once per cycle.  
/// The script works by detecting the player's presence via trigger colliders,  
/// and adjusting the local positions of the door panels accordingly.  
/// </summary>
/// <author>Chia Jia Cong Justin</author>
/// <date>10/8/2025</date>
/// <StudentID>S10266690C</StudentID>

using UnityEngine;

/// <summary>
/// Manages the opening and closing of a two-panel sliding door,
/// including movement and associated audio playback when the player enters or exits a trigger area.
/// </summary>
public class SlidingDoorBehaviour : MonoBehaviour
{
    /// <summary>
    /// The left door panel's transform.  
    /// This transform will slide forward when the door opens.
    /// </summary>
    public Transform doorL;

    /// <summary>
    /// The right door panel's transform.  
    /// This transform will slide backward when the door opens.
    /// </summary>
    public Transform doorR;

    /// <summary>
    /// The distance each door panel will move when opening.
    /// </summary>
    public float slideDistance = 1.5f;

    /// <summary>
    /// The speed at which the door panels slide open or closed.
    /// </summary>
    public float slideSpeed = 2f;

    /// <summary>
    /// Audio clip played when the doors open.
    /// </summary>
    public AudioClip openSound;

    /// <summary>
    /// Audio clip played when the doors close.
    /// </summary>
    public AudioClip closeSound;

    /// <summary>
    /// The closed position of the left door panel in local space.
    /// </summary>
    private Vector3 doorLClosedPos;

    /// <summary>
    /// The open position of the left door panel in local space.
    /// </summary>
    private Vector3 doorLOpenPos;

    /// <summary>
    /// The closed position of the right door panel in local space.
    /// </summary>
    private Vector3 doorRClosedPos;

    /// <summary>
    /// The open position of the right door panel in local space.
    /// </summary>
    private Vector3 doorROpenPos;

    /// <summary>
    /// Indicates whether the player is currently within the trigger area.
    /// </summary>
    private bool isPlayerInside = false;

    /// <summary>
    /// The AudioSource component used to play open and close sounds.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// Prevents the open sound from being played multiple times in succession.
    /// </summary>
    private bool hasPlayedOpenSound = false;

    /// <summary>
    /// Prevents the close sound from being played multiple times in succession.
    /// </summary>
    private bool hasPlayedCloseSound = true;

    /// <summary>
    /// Initializes door positions and gets the AudioSource component.
    /// </summary>
    void Start()
    {
        doorLClosedPos = doorL.localPosition;
        doorRClosedPos = doorR.localPosition;

        doorLOpenPos = doorLClosedPos + Vector3.forward * slideDistance;
        doorROpenPos = doorRClosedPos + Vector3.back * slideDistance;

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Smoothly opens or closes the doors each frame based on the player's presence.
    /// </summary>
    void Update()
    {
        if (isPlayerInside)
        {
            doorL.localPosition = Vector3.Lerp(doorL.localPosition, doorLOpenPos, Time.deltaTime * slideSpeed);
            doorR.localPosition = Vector3.Lerp(doorR.localPosition, doorROpenPos, Time.deltaTime * slideSpeed);
        }
        else
        {
            doorL.localPosition = Vector3.Lerp(doorL.localPosition, doorLClosedPos, Time.deltaTime * slideSpeed);
            doorR.localPosition = Vector3.Lerp(doorR.localPosition, doorRClosedPos, Time.deltaTime * slideSpeed);
        }
    }

    /// <summary>
    /// Detects when the player enters the trigger area, opening the doors and playing the open sound if needed.
    /// </summary>
    /// <param name="other">The collider that entered the trigger zone.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;

            if (!hasPlayedOpenSound && openSound)
            {
                audioSource.PlayOneShot(openSound);
                hasPlayedOpenSound = true;
                hasPlayedCloseSound = false;
            }
        }
    }

    /// <summary>
    /// Detects when the player exits the trigger area, closing the doors and playing the close sound if needed.
    /// </summary>
    /// <param name="other">The collider that exited the trigger zone.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;

            if (!hasPlayedCloseSound && closeSound)
            {
                audioSource.PlayOneShot(closeSound);
                hasPlayedCloseSound = true;
                hasPlayedOpenSound = false;
            }
        }
    }
}

