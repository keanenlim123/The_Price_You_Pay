/// <summary>
/// EnvyBehaviour.cs  
/// <summary>
/// AI behavior for the "Envy" enemy, which can patrol, chase the player, and perform a jumpscare.
/// Includes movement, animations, audio, and respawn handling.
/// </summary>
/// <author> Keanen Lim Xi En </author>
/// <date> 10/8/2025 </date>
/// <StudentID> S10270417C </StudentID>
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnvyBehaviour : MonoBehaviour
{
    /// <summary>
    /// States that Envy can be in.
    /// </summary>
    public enum EnemyState { Idle, Patrol, Chase, Jumpscare }

    /// <summary>
    /// The current active state of the enemy.
    /// </summary>
    public EnemyState currentState;

    [Header("Patrol Settings")]
    /// <summary>
    /// List of waypoints the enemy will patrol between.
    /// </summary>
    public Transform[] patrolPoints;

    /// <summary>
    /// The index of the current patrol point.
    /// </summary>
    private int patrolIndex;

    [Header("Player Reference")]
    /// <summary>
    /// The player's transform reference.
    /// </summary>
    public Transform player;

    /// <summary>
    /// The enemy's visible 3D model GameObject.
    /// </summary>
    public GameObject playermodel;

    [Header("Chase Settings")]
    /// <summary>
    /// Distance at which the enemy starts chasing the player.
    /// </summary>
    public float chaseRange = 10f;

    /// <summary>
    /// Distance at which the enemy catches the player and triggers a jumpscare.
    /// </summary>
    public float catchDistance = 2f;

    [Header("Movement")]
    /// <summary>
    /// Movement speed while sprinting.
    /// </summary>
    public float sprintSpeed = 10f;

    [Header("Idle & Wait Timers")]
    /// <summary>
    /// Timer used for idle waiting.
    /// </summary>
    public float waitTimer;

    /// <summary>
    /// Duration to wait at patrol points (used if a fixed wait is needed).
    /// </summary>
    public float waitDuration = 2f;

    /// <summary>
    /// Minimum random idle time between patrol points.
    /// </summary>
    public float minIdleTime = 1f;

    /// <summary>
    /// Maximum random idle time between patrol points.
    /// </summary>
    public float maxIdleTime = 3f;

    /// <summary>
    /// Whether the enemy is currently in a waiting state.
    /// </summary>
    private bool isWaiting = false;

    [Header("Components")]
    /// <summary>
    /// NavMeshAgent component for pathfinding.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Animator component for controlling animations.
    /// </summary>
    private Animator animator;

    [Header("Jumpscare Settings")]
    /// <summary>
    /// Camera to activate during the jumpscare.
    /// </summary>
    public GameObject camera1;

    /// <summary>
    /// Lighting to enable during the jumpscare.
    /// </summary>
    public GameObject lighting;

    /// <summary>
    /// Tracks whether the jumpscare has already been triggered to avoid repetition.
    /// </summary>
    private bool isJumpscareTriggered = false;

    [Header("Patrol Randomization")]
    /// <summary>
    /// Randomly chosen patrol point index for initial start.
    /// </summary>
    int randNum;

    [Header("Audio")]
    /// <summary>
    /// Footsteps audio source (looped during patrol/chase).
    /// </summary>
    public AudioSource footstepsAudio;

    /// <summary>
    /// Audio source for the jumpscare sound.
    /// </summary>
    public AudioSource jumpscareAudio;

    /// <summary>
    /// Initializes the enemy's components, randomizes patrol start, and sets up audio.
    /// </summary>
    void Start()
    {
        randNum = Random.Range(0, patrolPoints.Length);
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = sprintSpeed;

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing from " + gameObject.name);
            enabled = false;
        }

        currentState = EnemyState.Patrol;
        patrolIndex = randNum;
        animator.SetTrigger("sprint");

        // Ensure footsteps loop is disabled at start
        if (footstepsAudio != null)
        {
            footstepsAudio.loop = true;
            footstepsAudio.Stop();
        }
    }

    /// <summary>
    /// Handles state logic, transitions, and audio updates every frame.
    /// </summary>
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 🔊 Handle footsteps volume based on proximity
        if (footstepsAudio != null && currentState != EnemyState.Jumpscare)
        {
            if (currentState == EnemyState.Chase || currentState == EnemyState.Patrol)
            {
                if (!footstepsAudio.isPlaying) footstepsAudio.Play();
                float volume = Mathf.Clamp01(1f - (distanceToPlayer / chaseRange));
                footstepsAudio.volume = volume;
            }
            else
            {
                if (footstepsAudio.isPlaying) footstepsAudio.Stop();
            }
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.ResetTrigger("sprint");
                animator.ResetTrigger("jumpscare");
                animator.SetTrigger("idle");

                if (distanceToPlayer < chaseRange)
                {
                    currentState = EnemyState.Chase;
                }
                else
                {
                    Patrol();
                }
                break;

            case EnemyState.Patrol:
                agent.speed = sprintSpeed;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("sprint"))
                {
                    animator.ResetTrigger("idle");
                    animator.SetTrigger("sprint");
                }

                Patrol();

                if (distanceToPlayer < chaseRange)
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                agent.speed = sprintSpeed;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Sprint"))
                {
                    animator.ResetTrigger("idle");
                    animator.SetTrigger("sprint");
                }

                agent.SetDestination(player.position);

                if (distanceToPlayer > chaseRange * 1.5f)
                {
                    currentState = EnemyState.Patrol;
                }

                if (distanceToPlayer < catchDistance)
                    currentState = EnemyState.Jumpscare;
                break;

            case EnemyState.Jumpscare:
                if (!isJumpscareTriggered)
                {
                    isJumpscareTriggered = true;
                    agent.isStopped = true;

                    animator.ResetTrigger("sprint");
                    animator.ResetTrigger("idle");
                    animator.SetTrigger("jumpscare");

                    camera1.SetActive(true);
                    lighting.SetActive(true);
                    playermodel.SetActive(false);

                    // 🔊 Play jumpscare sound
                    if (jumpscareAudio != null)
                        jumpscareAudio.Play();

                    // Stop footsteps when jumpscare starts
                    if (footstepsAudio != null && footstepsAudio.isPlaying)
                        footstepsAudio.Stop();

                    StartCoroutine(HandleJumpscare());
                }
                break;
        }
    }

    /// <summary>
    /// Patrols between points with idle waiting between movements.
    /// </summary>
    void Patrol()
    {
        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isWaiting = true;
                waitTimer = Random.Range(minIdleTime, maxIdleTime); // <- Here!
                animator.SetTrigger("idle");
                currentState = EnemyState.Idle;
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0f)
            {
                patrolIndex = Random.Range(0, patrolPoints.Length);
                isWaiting = false;
                animator.SetTrigger("sprint");
                currentState = EnemyState.Patrol;
            }
        }
    }

    /// <summary>
    /// Handles the jumpscare sequence, player respawn, and reset of AI state.
    /// </summary>
    private IEnumerator HandleJumpscare()
    {
        yield return new WaitForSeconds(1f); // Jumpscare plays

        camera1.SetActive(false);
        lighting.SetActive(false);
        playermodel.SetActive(true);

        PlayerBehaviour pb = player.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            pb.Respawn();
        }

        yield return new WaitForSeconds(0.5f); // Prevent instant re-catch

        agent.isStopped = false;
        currentState = EnemyState.Patrol;

        // Trigger walk cleanly
        animator.ResetTrigger("jumpscare");
        yield return null;
        animator.SetTrigger("sprint");

        isJumpscareTriggered = false;
    }
}
