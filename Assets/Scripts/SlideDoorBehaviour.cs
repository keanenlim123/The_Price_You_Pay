using UnityEngine;

public class SlidingDoorBehaviour : MonoBehaviour
{
    public Transform doorL;
    public Transform doorR;

    public float slideDistance = 1.5f;
    public float slideSpeed = 2f;

    public AudioClip openSound;
    public AudioClip closeSound;

    private Vector3 doorLClosedPos;
    private Vector3 doorLOpenPos;
    private Vector3 doorRClosedPos;
    private Vector3 doorROpenPos;

    private bool isPlayerInside = false;

    private AudioSource audioSource;
    private bool hasPlayedOpenSound = false;
    private bool hasPlayedCloseSound = true;

    void Start()
    {
        doorLClosedPos = doorL.localPosition;
        doorRClosedPos = doorR.localPosition;

        doorLOpenPos = doorLClosedPos + Vector3.forward * slideDistance;
        doorROpenPos = doorRClosedPos + Vector3.back * slideDistance;

        audioSource = GetComponent<AudioSource>();
    }

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
