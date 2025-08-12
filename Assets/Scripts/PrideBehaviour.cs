using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class PrideBehaviour : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Jumpscare }
    public EnemyState currentState;

    public Transform[] patrolPoints;
    private int patrolIndex;

    public Transform player;

    public GameObject playermodel;
    public float chaseRange = 10f;
    public float catchDistance = 2f;

    public float walkSpeed = 2f;
    public float sprintSpeed = 5f;

    public float waitTimer;
    public float waitDuration = 2f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    private bool isWaiting = false;

    private NavMeshAgent agent;
    private Animator animator;

    public GameObject camera1;
    public GameObject lighting;
    private bool isJumpscareTriggered = false;

    public GameObject FootPrints;
    public int FootChance = 1;
    public PlayerBehaviour playerBehaviour;

    // 🔊 Added Audio
    public AudioSource footstepsAudio;
    public AudioSource jumpscareAudio;
    public float maxFootstepVolume = 1f;
    public float footstepVolumeDistance = 10f;

    int randNum;

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

        // Setup footsteps loop
        if (footstepsAudio != null)
        {
            footstepsAudio.loop = true;
            footstepsAudio.Play();
        }
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 🔊 Update footstep volume
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

                    // 🔊 Play jumpscare sound
                    if (jumpscareAudio != null)
                        jumpscareAudio.Play();

                    StartCoroutine(HandleJumpscare());
                }
                break;
        }
    }

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

                if (FootStepsBehaviour.footstepamount > 5 && FootStepsBehaviour.footstepamount < 10)
                {
                    if (FootPrints != null)
                    {
                        Vector3 spawnPos = patrolPoints[patrolIndex].position;
                        spawnPos.y -= 0f;

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
