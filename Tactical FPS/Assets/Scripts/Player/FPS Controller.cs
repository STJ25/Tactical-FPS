using UnityEngine;
using UnityEngine.InputSystem; // New Input System

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Sprinting,
        Jumping,
        InAir
    }

    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Ground Check Settings")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("References")]
    public Transform orientation;  // assign same orientation used by PlayerCamera
    [SerializeField] private PlayerInput playerInput;

    [Header("Debug State")]
    [SerializeField] private PlayerState currentState; // Visible in Inspector

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    // New Input System variables
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction sprintAction;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        // Reference actions by name (must match your Input Action asset)
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        sprintAction = playerInput.actions["Sprint"];
    }

    void Update()
    {
        HandleGroundCheck();
        HandleMovement();
        HandleGravityAndJump();
        UpdateState();
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        float x = input.x;
        float z = input.y;

        // Move relative to camera orientation
        Vector3 move = orientation.right * x + orientation.forward * z;

        float currentSpeed = sprintAction.IsPressed() ? sprintSpeed : walkSpeed;

        controller.Move(move.normalized * currentSpeed * Time.deltaTime);
    }

    void HandleGravityAndJump()
    {
        if (jumpAction.WasPressedThisFrame() && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateState()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        if (!isGrounded)
        {
            currentState = velocity.y > 0 ? PlayerState.Jumping : PlayerState.InAir;
        }
        else if (input == Vector2.zero)
        {
            currentState = PlayerState.Idle;
        }
        else if (sprintAction.IsPressed())
        {
            currentState = PlayerState.Sprinting;
        }
        else
        {
            currentState = PlayerState.Walking;
        }
    }
}
