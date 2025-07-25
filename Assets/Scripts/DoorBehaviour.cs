using UnityEngine;

public class DoorBehaviour : MonoBehaviour
{
    private bool isOpened = false;
    private AudioSource doorAudioSource;

    public float openSpeed = 2f;
    private Quaternion targetRotation;

    void Start()
    {
        doorAudioSource = GetComponent<AudioSource>();
        targetRotation = transform.rotation;
    }

    void Update()
    {
        // Smoothly rotate towards target rotation if door is opened
        if (isOpened)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor()
    {
        if (isOpened) return;

        isOpened = true;
        doorAudioSource.Play();

        // Rotate 90 degrees on Y axis
        targetRotation = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 270f, 0));
    }
}
