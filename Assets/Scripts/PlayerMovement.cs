using UnityEngine;
using TMPro; // Add this namespace for TextMeshPro

public class PlayerMovement : MonoBehaviour
{
    public Joystick joystick;
    public float speed = 10f;
    public float acceleration = 15f;
    private Animator animator;
    private Rigidbody rb;
    private Vector3 movementDirection;

    // Change this to TextMeshProUGUI for TextMeshPro
    public TextMeshProUGUI gameOverText; // For TextMeshProUGUI

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Enable Rigidbody interpolation for smoother movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // Prevents unwanted physics-based rotation

        // Hide the Game Over text at the start
        gameOverText.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = joystick.GetInputVector();
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // Apply smooth movement with physics
        Vector3 desiredVelocity = movementDirection * speed;
        Vector3 force = (desiredVelocity - rb.linearVelocity) * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);

        // Only rotate if the player is moving AND is grounded
        if (movementDirection.sqrMagnitude > 0.01f && IsGrounded())
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        else
        {
            // Stop unwanted spinning
            rb.angularVelocity = Vector3.zero;
        }

        // Handle animation
        animator.SetBool("Run", movementDirection.sqrMagnitude > 0.01f);
    }

    // Raycast to check if the player is on solid ground
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    // Trigger Game Over when player falls off
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone")) // Assuming you have a DeathZone trigger
        {
            // Show Game Over text
            gameOverText.gameObject.SetActive(true);

            // Optionally, stop player movement (freeze Rigidbody)
            rb.linearVelocity = Vector3.zero; // Stops the player completely
        }
    }
}
