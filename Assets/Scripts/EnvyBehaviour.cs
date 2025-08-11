using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnvyBehaviour : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Jumpscare }
    public EnemyState currentState;

    public Transform[] patrolPoints;
    private int patrolIndex;

    public Transform player;

    public GameObject playermodel;
    public float chaseRange = 10f;
    public float catchDistance = 2f;
    public float sprintSpeed = 10f;

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
    int randNum;

    // 🔊 Added audio sources
    public AudioSource footstepsAudio;
    public AudioSource jumpscareAudio;

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
