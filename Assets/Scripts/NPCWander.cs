
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

    private NavMeshAgent agent;
    private Animator animator;

    private bool canMove = true;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        StartCoroutine(Wander());
    }

    private void Update()
    {
        if (animator != null)
        {
            bool moving = canMove &&
                          agent.velocity.sqrMagnitude > 0.05f;

            animator.SetBool("IsMoving", moving);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;
        agent.isStopped = !enabled;

        if (!enabled && animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }

    private IEnumerator Wander()
    {
        while (true)
        {
            if (!canMove)
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
                {
                    yield return null;
                }

                if (agent.hasPath &&
                    agent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    while (canMove && agent.remainingDistance > agent.stoppingDistance + 0.2f)
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
