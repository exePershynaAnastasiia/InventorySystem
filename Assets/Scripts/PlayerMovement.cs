using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    [Header("Jump")]
    public float jumpForce = 5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 1.1f;

    private Rigidbody rb;
    private Vector2 moveInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            moveInput.y += 1;

        if (Keyboard.current.sKey.isPressed)
            moveInput.y -= 1;

        if (Keyboard.current.aKey.isPressed)
            moveInput.x -= 1;

        if (Keyboard.current.dKey.isPressed)
            moveInput.x += 1;

        moveInput = moveInput.normalized;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && IsGrounded())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (cameraTransform == null)
            return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection =
            forward * moveInput.y +
            right * moveInput.x;

        if (moveDirection.magnitude > 0.1f)
        {
            moveDirection.Normalize();

            Vector3 velocity = rb.linearVelocity;

            rb.linearVelocity = new Vector3(
                moveDirection.x * moveSpeed,
                velocity.y,
                moveDirection.z * moveSpeed
            );

            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.linearVelocity = new Vector3(
                0f,
                rb.linearVelocity.y,
                0f
            );
        }
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            0f,
            rb.linearVelocity.z
        );

        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }
}