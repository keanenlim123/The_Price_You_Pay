using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GreedBehaviour : MonoBehaviour
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

    public float shelfKnockRange = 2f;
    private bool isJumpscareTriggered = false;
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

        currentState = EnemyState.Patrol;
        patrolIndex = randNum;
        animator.SetTrigger("walk");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

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
        DetectNearbyShelves();
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
        animator.SetTrigger("walk");

        isJumpscareTriggered = false;
    }

    void DetectNearbyShelves()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, shelfKnockRange);

        foreach (var col in nearbyObjects)
        {
            if (col.CompareTag("Shelf"))
            {
                ShelfBehaviour shelf = col.GetComponent<ShelfBehaviour>();
                Debug.Log($"{gameObject.name} detected shelf {shelf.gameObject.name}, knocked down? {shelf.IsKnockedDown()}");

                if (!shelf.IsKnockedDown())
                {
                    shelf.KnockDown();
                    Debug.Log("Enemy knocked down a shelf!");
                }
            }
        }
    }
}