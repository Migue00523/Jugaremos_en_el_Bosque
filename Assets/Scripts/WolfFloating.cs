using UnityEngine;

public class WolfFloating : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float amplitude = 0.25f;
    [SerializeField] private float frequency = 1.5f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float newY = startPosition.y +
                     Mathf.Sin(Time.time * frequency) * amplitude;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}
