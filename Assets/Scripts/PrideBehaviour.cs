/// <summary>
/// PrideBehaviour.cs  
/// This script controls the AI behavior of the "Pride" monster in the game.  
/// Pride operates under a finite state machine with four states: Idle, Patrol, Chase, and Jumpscare.  
/// While patrolling, Pride may spawn muddy footprints if the player has nearly completed their cleaning objective,  
/// increasing the difficulty. If Pride detects the player within a certain range, it transitions to Chase mode,  
/// moving faster to pursue them. Upon close contact, Pride triggers a jumpscare sequence, temporarily halting  
/// the player’s progress.  
/// The script manages AI navigation using Unity's NavMeshAgent, controls animations, spawns additional objectives  
/// (footprints), and integrates audio feedback for footsteps and jumpscares.  
/// </summary>
/// <author>Chia Jia Cong Justin</author>
/// <date>10/8/2025</date>
/// <StudentID>S10266690C</StudentID>

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class PrideBehaviour : MonoBehaviour
{
    /// <summary>
    /// The different AI states for the Pride enemy.
    /// </summary>
    public enum EnemyState { Idle, Patrol, Chase, Jumpscare }

    /// <summary>
    /// Current active state of the Pride enemy.
    /// </summary>
    public EnemyState currentState;

    /// <summary>
    /// Array of patrol points that Pride moves between while patrolling.
    /// </summary>
    public Transform[] patrolPoints;

    /// <summary>
    /// Index of the current patrol point being targeted.
    /// </summary>
    private int patrolIndex;

    /// <summary>
    /// Reference to the player's Transform for tracking and chasing.
    /// </summary>
    public Transform player;

    /// <summary>
    /// Reference to the player's 3D model GameObject for visibility control during jumpscares.
    /// </summary>
    public GameObject playermodel;

    /// <summary>
    /// The detection range within which Pride starts chasing the player.
    /// </summary>
    public float chaseRange = 10f;

    /// <summary>
    /// Distance threshold at which Pride will trigger a jumpscare when close to the player.
    /// </summary>
    public float catchDistance = 2f;

    /// <summary>
    /// Movement speed while patrolling.
    /// </summary>
    public float walkSpeed = 2f;

    /// <summary>
    /// Movement speed while chasing the player.
    /// </summary>
    public float sprintSpeed = 5f;

    /// <summary>
    /// Timer for how long Pride waits at a patrol point.
    /// </summary>
    public float waitTimer;

    /// <summary>
    /// Duration to wait at a patrol point.
    /// </summary>
    public float waitDuration = 2f;

    /// <summary>
    /// Minimum random idle wait time.
    /// </summary>
    public float minIdleTime = 1f;

    /// <summary>
    /// Maximum random idle wait time.
    /// </summary>
    public float maxIdleTime = 3f;

    /// <summary>
    /// Flag indicating whether Pride is currently waiting.
    /// </summary>
    private bool isWaiting = false;

    /// <summary>
    /// Reference to the NavMeshAgent for navigation and pathfinding.
    /// </summary>
    private NavMeshAgent agent;

    /// <summary>
    /// Reference to the Animator for controlling Pride’s animations.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// Reference to the jumpscare camera for visual effect.
    /// </summary>
    public GameObject camera1;

    /// <summary>
    /// Reference to lighting used during jumpscare sequences.
    /// </summary>
    public GameObject lighting;

    /// <summary>
    /// Flag to ensure jumpscare is triggered only once at a time.
    /// </summary>
    private bool isJumpscareTriggered = false;

    /// <summary>
    /// Footprint prefab to spawn as an additional player objective.
    /// </summary>
    public GameObject FootPrints;

    /// <summary>
    /// Chance value (not fully used in this script) for spawning footprints.
    /// </summary>
    public int FootChance = 1;

    /// <summary>
    /// Reference to the PlayerBehaviour script for updating objective counters.
    /// </summary>
    public PlayerBehaviour playerBehaviour;

    /// <summary>
    /// Audio source for Pride’s footsteps.
    /// </summary>
    public AudioSource footstepsAudio;

    /// <summary>
    /// Audio source for Pride’s jumpscare sound.
    /// </summary>
    public AudioSource jumpscareAudio;

    /// <summary>
    /// Maximum volume of the footsteps sound.
    /// </summary>
    public float maxFootstepVolume = 1f;

    /// <summary>
    /// Distance at which footstep audio starts fading.
    /// </summary>
    public float footstepVolumeDistance = 10f;

    /// <summary>
    /// Random number for selecting initial patrol point.
    /// </summary>
    int randNum;

    /// <summary>
    /// Initializes the Pride enemy, selecting a random patrol point, setting animation triggers,
    /// and starting footstep audio playback.
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
        if (playerBehaviour == null && player != null)
        {
            playerBehaviour = player.GetComponent<PlayerBehaviour>();
            if (playerBehaviour == null)
            {
                Debug.LogWarning("PlayerBehaviour component not found on player.");
            }
        }

        currentState = EnemyState.Patrol;
        patrolIndex = randNum;
        animator.SetTrigger("walk");

        if (footstepsAudio != null)
        {
            footstepsAudio.loop = true;
            footstepsAudio.Play();
        }
    }

    /// <summary>
    /// Handles AI state updates, distance checks to the player, audio adjustments,
    /// and transitions between states based on conditions each frame.
    /// </summary>
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (footstepsAudio != null)
        {
            float volume = Mathf.Clamp01(1 - (distanceToPlayer / footstepVolumeDistance));
            footstepsAudio.volume = volume * maxFootstepVolume;
            footstepsAudio.mute = (currentState != EnemyState.Patrol && currentState != EnemyState.Chase);
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

                    if (jumpscareAudio != null)
                        jumpscareAudio.Play();

                    StartCoroutine(HandleJumpscare());
                }
                break;
        }
    }

    /// <summary>
    /// Moves Pride to patrol points, waits at each, and may spawn footprints if the player
    /// has nearly completed their cleaning objective.
    /// </summary>
    void Patrol()
    {
        if (!isWaiting)
        {
            agent.SetDestination(patrolPoints[patrolIndex].position);

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                isWaiting = true;
                waitTimer = Random.Range(minIdleTime, maxIdleTime);
                animator.SetTrigger("idle");
                currentState = EnemyState.Idle;

                if (FootStepsBehaviour.footstepamount > 8 && FootStepsBehaviour.footstepamount < 10)
                {
                    if (FootPrints != null)
                    {
                        Vector3 spawnPos = patrolPoints[patrolIndex].position;
                        Instantiate(FootPrints, spawnPos, Quaternion.identity);
                        Debug.Log("Footprints spawned at idle point: " + patrolIndex);

                        FootStepsBehaviour.footstepamount--;

                        if (playerBehaviour != null)
                        {
                            playerBehaviour.DecreaseFootprintsCleanedCount();
                        }
                    }
                }
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
            }
        }
    }

    /// <summary>
    /// Handles the jumpscare sequence: plays animation, shows jumpscare camera,
    /// disables player model, and respawns player after a delay.
    /// </summary>
    private IEnumerator HandleJumpscare()
    {
        yield return new WaitForSeconds(1f);

        camera1.SetActive(false);
        lighting.SetActive(false);
        playermodel.SetActive(true);

        PlayerBehaviour pb = player.GetComponent<PlayerBehaviour>();
        if (pb != null)
        {
            pb.Respawn();
        }

        yield return new WaitForSeconds(0.5f);

        agent.isStopped = false;
        currentState = EnemyState.Patrol;

        animator.ResetTrigger("jumpscare");
        yield return null;
        animator.SetTrigger("walk");

        isJumpscareTriggered = false;
    }
}