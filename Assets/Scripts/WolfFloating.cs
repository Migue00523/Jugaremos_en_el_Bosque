
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WolfFloating : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float amplitude = 0.2f;
    [SerializeField] private float frequency = 1.5f;

    private NavMeshAgent agent;
    private float initialOffset;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialOffset = agent.baseOffset;
    }

    private void Update()
    {
        float floating = Mathf.Sin(Time.time * frequency) * amplitude;

        agent.baseOffset = initialOffset + floating;
    }
}
