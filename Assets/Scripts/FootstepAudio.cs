using UnityEngine;

public class FootstepAudio : MonoBehaviour
{
    public AudioClip[] footstepClips; // drag multiple footstep sounds here
    public float stepInterval = 0.5f; // time between steps
    public float moveThreshold = 0.1f; // how much movement counts as "walking"

    private AudioSource audioSource;
    private CharacterController controller;
    private float stepTimer;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controller = GetComponent<CharacterController>();
    }

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
            stepTimer = 0f; // reset timer when not moving
        }
    }

    void PlayFootstep()
    {
        if (footstepClips.Length > 0)
        {
            int index = Random.Range(0, footstepClips.Length);
            audioSource.PlayOneShot(footstepClips[index]);
        }
    }
}
