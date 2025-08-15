/// <summary>
/// DoorBehaviour.cs
/// Controls the opening behavior of a door, including smooth rotation and sound effects.
/// </summary>
/// <author> Keanen Lim Xi En </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10270417C </StudentID>
using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
     /// <summary>
    /// Tracks whether the door has already been opened.
    /// </summary>
    private bool isOpened = false;

    /// <summary>
    /// The audio source used to play the door opening sound.
    /// </summary>
    private AudioSource doorAudioSource;

    /// <summary>
    /// Speed at which the door rotates to its open position.
    /// </summary>
    public float openSpeed = 2f;

    /// <summary>
    /// The target rotation the door should reach when opened.
    /// </summary>
    private Quaternion targetRotation;

    /// <summary>
    /// Initializes the audio source and sets the initial rotation.
    /// </summary>
    void Start()
    {
        doorAudioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
    }

    /// <summary>
    /// Smoothly rotates the door towards its target rotation if it has been opened.
    /// </summary>
    void Update()
    {
        // Smoothly rotate towards target rotation if door is opened
        if (isOpened)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    /// <summary>
    /// Opens the door by setting the target rotation and playing the opening sound.
    /// Will not run again if the door is already opened.
    /// </summary>
    public void OpenDoor()
    {
        if (isOpened) return;

        isOpened = true;
        doorAudioSource.Play();

        // Rotate 270 degrees on Y axis (clockwise from current rotation)
        targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 270f, 0));
    }
}
