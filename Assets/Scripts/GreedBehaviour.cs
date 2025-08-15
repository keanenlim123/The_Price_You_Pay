/// <summary>
/// GreedBehaviour.cs  
/// This script controls the AI behaviour of the Greed enemy, including movement, detection,
/// and interactions with the player. It manages how Greed chases, reacts to player actions,
/// and transitions between behaviour states.
/// </summary>
/// <author> Lim Xue Zhi Conan </author>
/// <date> 11/8/2025 </date>
/// <StudentID> S10269214H </StudentID>

using UnityEngine;
using UnityEngine.AI;
using System.Collections;

/// <summary>
/// Controls the AI behaviour of the Greed enemy, including patrolling, chasing the player, and executing a jumpscare.
/// Handles movement between patrol points, detecting the player within chase range,
/// adjusting footstep audio volume dynamically, and triggering game events upon catching the player.
/// </summary>
public class GreedBehaviour : MonoBehaviour
{
    /// <summary>Represents the current behaviour state of Greed.</summary>
    public enum EnemyState { Idle, Patrol, Chase, Jumpscare }
    /// <summary>The current state of the enemy.</summary>
    public EnemyState currentState;

    [Header("Patrol Settings")]
    /// <summary>List of patrol points for Greed to move between.</summary>
    public Transform[] patrolPoints;
    /// <summary>The index of the current patrol point.</summary>
    private int patrolIndex;

    [Header("Player Detection")]
    /// <summary>Reference to the player’s Transform.</summary>
    public Transform player;
    /// <summary>The player's in-game model.</summary>
    public GameObject playermodel;
    /// <summary>The distance at which Greed will start chasing the player.</summary>
    public float chaseRange = 10f;
    /// <summary>The distance at which Greed will trigger a jumpscare.</summary>
    public float catchDistance = 2f;

    [Header("Movement Speeds")]
    /// <summary>Walking speed of Greed during patrol.</summary>
    public float walkSpeed = 2f;
    /// <summary>Sprinting speed of Greed during chase.</summary>
    public float sprintSpeed = 5f;

    [Header("Idle & Wait Times")]
    /// <summary>Tracks how long Greed has been waiting at a patrol point.</summary>
    public float waitTimer;
    /// <summary>How long Greed waits at a patrol point before moving on.</summary>
    public float waitDuration = 2f;
    /// <summary>Minimum time Greed will idle between patrol points.</summary>
    public float minIdleTime = 1f;
    /// <summary>Maximum time Greed will idle between patrol points.</summary>
    public float maxIdleTime = 3f;
    /// <summary>Whether Greed is currently waiting between patrol points.</summary>
    private bool isWaiting = false;

    [Header("Components")]
    /// <summary>Reference to the NavMeshAgent controlling Greed's navigation.</summary>
    private NavMeshAgent agent;
    /// <summary>Reference to Greed's Animator for controlling animations.</summary>
    private Animator animator;

    [Header("Jumpscare Settings")]
    /// <summary>The camera used for the jumpscare sequence.</summary>
    public GameObject camera1;
    /// <summary>Lighting object used during the jumpscare.</summary>
    public GameObject lighting;
    /// <summary>Whether the jumpscare sequence has been triggered.</summary>
    private bool isJumpscareTriggered = false;

    [Header("Shelf Knockdown")]
    /// <summary>The range in which Greed can knock over shelves.</summary>
    public float shelfKnockRange = 2f;

    [Header("Random Patrol")]
    /// <summary>Random number used for selecting patrol points.</summary>
    private int randNum;

    [Header("Audio")]
    /// <summary>AudioSource for Greed's footsteps.</summary>
    public AudioSource footstepsAudio;
    /// <summary>AudioSource for Greed's jumpscare sound.</summary>
    public AudioSource jumpscareAudio;
    /// <summary>
    /// Initializes variables, sets a random patrol point, and ensures footsteps audio is looping.
    /// </summary>
    void Start()
    {
        randNum = Random.Range(0, patrolPoints.Length);
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = walkSpeed;

        if (agent == null)
        {
            Debug.LogError("NavMeshAgent is missing from " + gameObject.name);
            enabled = false;
        }

        currentState = EnemyState.Patrol;
        patrolIndex = randNum;
        animator.SetTrigger("walk");

        // Make sure footsteps loop if assigned
        if (footstepsAudio != null)
        {
            footstepsAudio.loop = true;
            footstepsAudio.Play();
        }
    }

    /// <summary>
    /// Updates Greed's behaviour each frame depending on the current state.
    /// Adjusts footstep volume dynamically based on player's distance.
    /// </summary>
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Adjust footsteps volume based on distance
        if (footstepsAudio != null)
        {
            float volume = Mathf.Clamp01(1 - (distanceToPlayer / chaseRange));
            footstepsAudio.volume = volume;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                animator.ResetTrigger("walk");
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
                agent.speed = walkSpeed;

                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    animator.ResetTrigger("idle");
                    animator.ResetTrigger("sprint");
                    animator.SetTrigger("walk");
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
                    animator.ResetTrigger("walk");
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

                    animator.ResetTrigger("walk");
                    animator.ResetTrigger("sprint");
                    animator.ResetTrigger("idle");
                    animator.SetTrigger("jumpscare");

                    camera1.SetActive(true);
                    lighting.SetActive(true);
                    playermodel.SetActive(false);

                    // Play jumpscare audio
                    if (jumpscareAudio != null)
                        jumpscareAudio.Play();

                    StartCoroutine(HandleJumpscare());
                }
                break;
        }
    }

    /// <summary>
    /// Handles Greed's movement between patrol points, including idle waiting periods.
    /// </summary>
    void Patrol()
    {
        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);
            Debug.Log($"Setting destination to patrol point {patrolIndex}: {patrolPoints[patrolIndex].position}");

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isWaiting = true;
                waitTimer = Random.Range(minIdleTime, maxIdleTime);
                animator.SetTrigger("idle");
                currentState = EnemyState.Idle;
                Debug.Log("Reached patrol point, idling.");
            }
        }
        else
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                patrolIndex = Random.Range(0, patrolPoints.Length);
                isWaiting = false;
                animator.SetTrigger("walk");
                currentState = EnemyState.Patrol;
                Debug.Log($"Moving to next patrol point: {patrolIndex}");
            }
        }
    }

    /// <summary>
    /// Handles the jumpscare sequence, including camera and lighting changes,
    /// resetting the player, and resuming patrol after a delay.
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
        animator.SetTrigger("walk");

        isJumpscareTriggered = false;
    }
}
