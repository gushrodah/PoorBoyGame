using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Animation")]
    public Animator animator;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float turnSmoothTime = 0.1f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;
    public Transform cameraTransform;

    [Header("Beat Jump")]
    public BeatTracker beatTracker;
    public float jumpCooldown = 0.5f;
    public float floatFallSpeed = -2f;
    public float floatResourceDrainRate = 10f;  // Resource units per second

    [Header("Beat Dash")]
    public KeyCode dashKey = KeyCode.LeftShift;
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.5f;
    public float flySpeed = 15f;
    public float flyResourceDrainRate = 20f;

    [Header("Resource")]
    public FloatResourceManager resourceManager;

    private CharacterController controller;
    private float turnSmoothVelocity;
    private Vector3 velocity;
    private bool isGrounded;
    private float lastJumpTime;
    private float lastDashTime;
    private Vector3 dashVelocity;
    private bool isDashing;
    private bool wasLastJumpOnBeat;
    private bool wasLastDashOnBeat;
    private Vector3 flyDirection;
    private bool isFlying;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        if (beatTracker == null)
        {
            beatTracker = FindObjectOfType<BeatTracker>();
        }
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }
        if (resourceManager == null)
        {
            resourceManager = FindObjectOfType<FloatResourceManager>();
        }
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private Vector3 GetMovementDirection(Vector3 inputDirection)
    {
        if (inputDirection.magnitude < 0.1f) return Vector3.zero;

        // Get camera forward and right vectors
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        
        // Project vectors onto the horizontal plane
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate move direction relative to camera
        return (forward * inputDirection.z + right * inputDirection.x).normalized;
    }

    private void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            wasLastJumpOnBeat = false;
            wasLastDashOnBeat = false;
            isFlying = false;
            if (animator != null)
            {
                animator.SetBool("Jump", false);
                animator.SetBool("Fly", false);
            }
        }

        // Movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveDir = GetMovementDirection(inputDirection);

        // Move the player
        if (moveDir != Vector3.zero)
        {
            // Rotate player to face movement direction
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(moveDir * moveSpeed * Time.deltaTime);
            if (animator != null)
            {
                //animator.SetBool("Grounded", false);
            }
        }
        else if (isGrounded)
        {
            if (animator != null)
            {
                //animator.SetBool("Grounded", true);
                animator.Play("Idle");
            }
        }

        // Jump on beat
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastJumpTime + jumpCooldown)
        {
            if (beatTracker.IsOnBeat())
            {
                velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
                lastJumpTime = Time.time;
                wasLastJumpOnBeat = true;
                resourceManager?.OnBeatActionPerformed();
                if (animator != null)
                {
                    animator.SetBool("Jump", true);
                }
            }
        }

        // Handle Dash and Flying
        if (Input.GetKeyDown(dashKey) && Time.time > lastDashTime + dashCooldown)
        {
            if (beatTracker.IsOnBeat())
            {
                Vector3 dashDirection;
                if (!isGrounded)
                {
                    // Combine controller's forward with camera's vertical component
                    Vector3 horizontalDir = transform.forward;
                    float verticalComponent = cameraTransform.forward.y;
                    dashDirection = new Vector3(horizontalDir.x, verticalComponent, horizontalDir.z);
                }
                else
                {
                    dashDirection = moveDir != Vector3.zero ? moveDir : transform.forward;
                }
                
                flyDirection = dashDirection.normalized;
                wasLastDashOnBeat = true;
                StartCoroutine(PerformDash(flyDirection));
                lastDashTime = Time.time;
                resourceManager?.OnBeatActionPerformed();
                if (animator != null)
                {
                    animator.SetBool("Fly", true);
                }
            }
        }

        // Handle Flying after dash
        if (wasLastDashOnBeat && Input.GetKey(dashKey) && resourceManager.HasEnoughResource(flyResourceDrainRate * Time.deltaTime))
        {
            isFlying = true;
            flyDirection = cameraTransform.forward.normalized;
            resourceManager.ConsumeResource(flyResourceDrainRate * Time.deltaTime);
            if (animator != null)
            {
                animator.SetBool("Fly", true);
            }
        }
        else
        {
            isFlying = false;
            if (animator != null)
            {
                animator.SetBool("Fly", false);
            }
        }

        // Apply movement and gravity
        if (isDashing)
        {
            controller.Move(dashVelocity * Time.deltaTime);
        }
        else if (isFlying)
        {
            controller.Move(flyDirection * flySpeed * Time.deltaTime);
        }
        else
        {
            if (!isGrounded && wasLastJumpOnBeat && Input.GetKey(KeyCode.Space) && velocity.y < 0 
                && resourceManager.HasEnoughResource(floatResourceDrainRate * Time.deltaTime))
            {
                velocity.y = floatFallSpeed;
                resourceManager.ConsumeResource(floatResourceDrainRate * Time.deltaTime);
            }
            else
            {
                velocity.y += gravity * Time.deltaTime;
            }
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private IEnumerator PerformDash(Vector3 direction)
    {
        isDashing = true;
        dashVelocity = direction * dashForce;
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

    }
}
