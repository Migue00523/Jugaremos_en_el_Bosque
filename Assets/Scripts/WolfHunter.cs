
using UnityEngine;
using UnityEngine.AI;

public class WolfHunter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerVisual;
    [SerializeField] private GameOverManager gameOverManager;
    [SerializeField] private Transform patrolCenter;

    [Header("Search Settings")]
    [SerializeField] private float patrolRadius = 35f;
    [SerializeField] private float viewDistance = 12f;
    [SerializeField] private float detectionDistance = 2f;
    [SerializeField] private float waitBetweenDestinations = 3f;

    private bool isHunting = false;
    private float destinationTimer;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    public void StartHunting()
    {
        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError("El lobo no está sobre el NavMesh.");
            return;
        }

        isHunting = true;
        agent.isStopped = false;
        destinationTimer = 0f;

        ChooseNewDestination();

        Debug.Log("¡El lobo comienza a buscar!");
    }

    private void Update()
    {
        if (!isHunting || agent == null)
            return;

        bool playerIsVisible =
            playerVisual != null &&
            playerVisual.activeInHierarchy;

        // Perseguir al jugador.
        if (playerIsVisible && player != null)
        {
            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distance <= detectionDistance)
            {
                agent.isStopped = true;

                if (gameOverManager != null)
                    gameOverManager.TriggerGameOver();

                return;
            }

            if (distance <= viewDistance)
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
                return;
            }
        }

        // Patrullaje: esperar hasta llegar al destino.
        if (agent.pathPending)
            return;

        if (agent.hasPath &&
            agent.remainingDistance > agent.stoppingDistance + 0.2f)
        {
            return;
        }

        // Solo espera cuando ya llegó al destino.
        destinationTimer += Time.deltaTime;

        if (destinationTimer >= waitBetweenDestinations)
        {
            ChooseNewDestination();
            destinationTimer = 0f;
        }
    }

    private void ChooseNewDestination()
    {
        Vector3 center = patrolCenter != null
            ? patrolCenter.position
            : transform.position;

        for (int i = 0; i < 15; i++)
        {
            Vector3 randomPoint = center + new Vector3(
                Random.Range(-patrolRadius, patrolRadius),
                0f,
                Random.Range(-patrolRadius, patrolRadius)
            );

            if (NavMesh.SamplePosition(
                randomPoint,
                out NavMeshHit hit,
                8f,
                NavMesh.AllAreas))
            {
                agent.isStopped = false;
                agent.SetDestination(hit.position);

                Debug.Log("Nuevo destino del lobo: " + hit.position);
                return;
            }
        }

        Debug.LogWarning("El lobo no encontró un nuevo destino.");
    }
}
