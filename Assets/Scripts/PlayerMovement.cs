using UnityEngine;
using TMPro;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    private Joystick joystick; // Dynamically assigned joystick
    public float speed = 10f;
    public float acceleration = 15f;
    public Animator animator;
    private Rigidbody rb;
    private Vector3 movementDirection;
    private TextMeshProUGUI gameOverText; // Dynamically assigned Game Over text

    void Start()
    {
        Debug.Log($"Player spawned! IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");

        if (!IsOwner) return; // Only the local player should set UI references

        // Find the joystick in the scene
        joystick = FindObjectOfType<Joystick>();

        // Find the Game Over text in the scene
        gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false); // Hide Game Over at start
        }

        rb = GetComponent<Rigidbody>();

        // Enable Rigidbody interpolation for smoother movement
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true; // Prevents unwanted physics-based rotation

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log(animator != null ? "Animator assigned!" : "Animator is still null!");
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || joystick == null) return; // Only the owner can move the player

        Vector2 moveInput = joystick.GetInputVector();
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // Apply smooth movement with physics
        Vector3 desiredVelocity = movementDirection * speed;
        Vector3 force = (desiredVelocity - rb.linearVelocity) * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);

        // Rotate player when moving
        if (movementDirection.sqrMagnitude > 0.01f && IsGrounded())
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        else
        {
            rb.angularVelocity = Vector3.zero; // Prevents unwanted spinning
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
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true); // Show Game Over text
            }

            // Stop player movement (freeze Rigidbody)
            rb.linearVelocity = Vector3.zero;
        }
    }
}
