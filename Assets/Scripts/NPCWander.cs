
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NPCWander : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float wanderRadius = 40f;
    [SerializeField] private float waitTime = 3f;
    [SerializeField] private float minWaitTime = 1f;

    [Header("References")]
    [SerializeField] private Transform wanderCenter;
    [SerializeField] private CharacterVisibility characterVisibility;

    private NavMeshAgent agent;
    private Animator animator;

    private bool canMove = true;
    private bool isHiding = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        if (characterVisibility == null)
            characterVisibility = GetComponent<CharacterVisibility>();
    }

    private void Start()
    {
        StartCoroutine(Wander());
    }

    private void Update()
    {
        if (animator != null && agent != null)
        {
            bool moving = canMove &&
                          agent.velocity.sqrMagnitude > 0.05f;

            animator.SetBool("IsMoving", moving);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        if (isHiding)
            return;

        canMove = enabled;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = !enabled;

        if (!enabled && animator != null)
            animator.SetBool("IsMoving", false);
    }

    public void HideInNearestBush()
    {
        if (isHiding)
            return;

        BushHidingSpot bush =
            BushHidingSpot.FindClosestBush(transform.position);

        if (bush == null)
        {
            Debug.LogWarning(name + " could not find a bush!");
            return;
        }

        Vector3 hidingPosition = bush.GetNextPosition();

        isHiding = true;
        canMove = true;

        StartCoroutine(MoveToHidingPosition(hidingPosition));
    }

    private IEnumerator MoveToHidingPosition(Vector3 destination)
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning(name + " is not on the NavMesh!");
            yield break;
        }

        agent.isStopped = false;
        agent.SetDestination(destination);

        while (agent.pathPending)
            yield return null;

        if (agent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning(name + " could not reach its hiding spot!");
            agent.ResetPath();
            agent.isStopped = true;
            canMove = false;
            yield break;
        }

        while (agent.remainingDistance >
               agent.stoppingDistance + 0.15f)
        {
            yield return null;
        }

        agent.isStopped = true;
        agent.ResetPath();

        canMove = false;

        if (animator != null)
            animator.SetBool("IsMoving", false);

        if (characterVisibility != null)
            characterVisibility.SetHidden(true);

        Debug.Log(name + " is hiding!");
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            if (!canMove || isHiding)
            {
                yield return null;
                continue;
            }

            Vector3 center = wanderCenter != null
                ? wanderCenter.position
                : transform.position;

            Vector3 randomDirection =
                Random.insideUnitSphere * wanderRadius;

            randomDirection += center;

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                randomDirection,
                out hit,
                5f,
                NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);

                while (agent.pathPending)
                    yield return null;

                if (agent.hasPath &&
                    agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    while (canMove &&
                           !isHiding &&
                           agent.remainingDistance >
                           agent.stoppingDistance + 0.2f)
                    {
                        yield return null;
                    }
                }
            }

            float wait = Random.Range(minWaitTime, waitTime);
            yield return new WaitForSeconds(wait);
        }
    }
}
