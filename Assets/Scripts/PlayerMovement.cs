using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float sprintSpeed = 11f;
    [SerializeField] private float acceleration = 14f; // higher = snappier starts and stops
        
    [Header("Jumping and Gravity")]
    [SerializeField] private float jumpHeight = 7f;
    [SerializeField] private float gravity = -28f;
    [SerializeField] private float fallGravityMultiplier = 1.4f; // punchier fall = snappier jump feel
    [SerializeField] private float groundedStickForce = -2f; // keeps isGrounded reliable on slopes
        
    [Header("Air Control")]
    [Range(0f, 1f)]
    [SerializeField] private float airControlFactor = 0.65f; // how much steering you have in the air

    private CharacterController controller;
    private Vector3 currentVelocity; // horizontal velocity, smoothed
    private float verticalVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 moveInput = ReadMoveInput();
        bool sprintHeld = Keyboard.current.leftShiftKey.isPressed;
        bool jumpPressed = Keyboard.current.spaceKey.wasPressedThisFrame;

        StickToGround();
        HandleHorizontalMovement(moveInput, sprintHeld);
        HandleVertical(jumpPressed);
        Vector3 fullVelocity = currentVelocity + Vector3.up * verticalVelocity;
        controller.Move(fullVelocity * Time.deltaTime);
    }

    private Vector2 ReadMoveInput()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) input.y += 1f;
        if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        if (Keyboard.current.dKey.isPressed) input.x += 1f;
        if (Keyboard.current.aKey.isPressed) input.x -= 1f;
        return Vector2.ClampMagnitude(input, 1f);
    }

    private void StickToGround()
    {
        //without this, isGrounded can flicker galse for a frame on flatground/steps
        if (controller.isGrounded && verticalVelocity < 0f)
            {
            verticalVelocity = groundedStickForce;
            }
    }

    private void HandleHorizontalMovement(Vector2 input, bool sprint)
    {
        Vector3 wishDir = transform.TransformDirection(new Vector3(input.x, 0f, input.y));
        float targetSpeed = sprint ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = wishDir * targetSpeed;
        
        float control = controller.isGrounded ? 1f : airControlFactor;
        // frame-rate independent smoothing (exponential decay) - snappy without being jittery
        float t = 1f - Mathf.Exp(-acceleration * control * Time.deltaTime);
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, t);
    }

    private void HandleVertical(bool jumpPressed)
    {
        if (jumpPressed && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        
        float appliedGravity = verticalVelocity < 0f ? gravity * fallGravityMultiplier : gravity;
        verticalVelocity += appliedGravity * Time.deltaTime;
    }
}
