
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.5f, -5f);
    [SerializeField] private float smoothSpeed = 4f;
    [SerializeField] private float lookHeight = 1.2f;

    private void LateUpdate()
    {
        if (target == null) return;

        // Calculate camera position relative to player's rotation
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Smooth camera movement
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );

        // Look at player
        Vector3 lookPosition = target.position + Vector3.up * lookHeight;

        transform.LookAt(lookPosition);
    }
}
