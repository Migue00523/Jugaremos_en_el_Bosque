
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float gravity = -20f;

    [Header("References")]
    [SerializeField] private VirtualJoystick joystick;
    [SerializeField] private Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    private Vector3 verticalVelocity;
    private bool canMove = true;

    public bool IsTryingToMove { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;

        // Mobile joystick
        if (joystick != null)
        {
            input = joystick.InputDirection;
        }

        // Keyboard testing in Unity Editor
        if (Keyboard.current != null)
        {
            Vector2 keyboardInput = Vector2.zero;

            if (Keyboard.current.wKey.isPressed) keyboardInput.y += 1;
            if (Keyboard.current.sKey.isPressed) keyboardInput.y -= 1;
            if (Keyboard.current.dKey.isPressed) keyboardInput.x += 1;
            if (Keyboard.current.aKey.isPressed) keyboardInput.x -= 1;

            if (keyboardInput != Vector2.zero)
                input = keyboardInput.normalized;
        }

        IsTryingToMove = input.magnitude > 0.15f;

        Move(input);
    }

    private void Move(Vector2 input)
    {
        if (!canMove)
            input = Vector2.zero;

        Vector3 direction = Vector3.zero;

        if (cameraTransform != null)
        {
            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            direction = forward * input.y + right * input.x;
        }
        else
        {
            direction = new Vector3(input.x, 0, input.y);
        }

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        controller.Move(direction.normalized * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity.y < 0)
        {
            verticalVelocity.y = -2f;
        }

        verticalVelocity.y += gravity * Time.deltaTime;

        controller.Move(verticalVelocity * Time.deltaTime);

        if (animator != null)
        {
            animator.SetBool("IsMoving", direction.magnitude > 0.1f);
        }
    }

    public void SetMovementEnabled(bool enabled)
    {
        canMove = enabled;

        if (!enabled && animator != null)
        {
            animator.SetBool("IsMoving", false);
        }
    }
}
