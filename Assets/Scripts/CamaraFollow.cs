using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 3.5f, -5f);
    [SerializeField] private float smoothSpeed = 4f;
    [SerializeField] private float lookHeight = 1.2f;

    [Header("Hidden Camera")]
    [SerializeField] private float sensitivity = 0.15f;
    [SerializeField] private float minPitch = 5f;
    [SerializeField] private float maxPitch = 55f;

    private bool lookMode = false;
    private float yaw;
    private float pitch = 20f;

    private void Start()
    {
        if (target != null)
            yaw = target.eulerAngles.y;
    }

    public void SetLookMode(bool enabled)
    {
        lookMode = enabled;

        if (enabled)
        {
            yaw = target.eulerAngles.y;
            pitch = 20f;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (lookMode)
        {
            HandleCameraInput();

            Quaternion orbitRotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 desiredPosition = target.position +
                                      orbitRotation * offset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            transform.LookAt(
                target.position + Vector3.up * lookHeight
            );
        }
        else
        {
            Vector3 desiredPosition = target.position +
                                      target.rotation * offset;

            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            transform.LookAt(
                target.position + Vector3.up * lookHeight
            );
        }
    }

    private void HandleCameraInput()
    {
        Vector2 delta = Vector2.zero;

        // Mobile touch
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            delta = Touchscreen.current.primaryTouch.delta.ReadValue();
        }
        // Mouse for testing in the Editor
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed)
        {
            delta = Mouse.current.delta.ReadValue();
        }

        yaw += delta.x * sensitivity;
        pitch -= delta.y * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }
}
