using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{
    private Joystick joystick;
    public float speed = 10f;
    public float acceleration = 15f;
    public Animator animator;
    private Rigidbody rb;
    private Vector3 movementDirection;
    private TextMeshProUGUI gameOverText;

    public Button pushButton;
    public float pushForce = 10f;
    public float pushRadius = 3f;
    private bool isPushing = false;
    public float pushCooldown = 3f; // Cooldown in seconds

    public bool canMove = false;

    private TextMeshProUGUI pushButtonText; // Timer text inside button
    private Color originalButtonColor;
    private Image buttonImage; // UI Image component of the button

    void Start()
    {
        Debug.Log($"Player spawned! IsOwner: {IsOwner}, OwnerClientId: {OwnerClientId}");

        if (!IsOwner) return;

        // Disable camera if this is not the local player
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
            playerCamera.enabled = false;
        else
            Debug.LogWarning("No camera found!");

        AudioListener listener = GetComponentInChildren<AudioListener>();
        if (listener != null)
            listener.enabled = false;
        else
            Debug.LogWarning("No AudioListener found!");

        gameObject.name = $"Player_{OwnerClientId}";
        Debug.Log($"[Player] Assigned name: {gameObject.name}");

        joystick = FindObjectOfType<Joystick>();
        gameOverText = GameObject.Find("GameOverText")?.GetComponent<TextMeshProUGUI>();

        if (gameOverText != null) gameOverText.gameObject.SetActive(false);

        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            Debug.Log(animator != null ? "Animator assigned!" : "Animator is still null!");
        }

        // Find or create the push button and assign event
        pushButton = GameObject.Find("FartButton")?.GetComponent<Button>();
        if (pushButton != null)
        {
            pushButton.onClick.AddListener(AttemptPush);
            buttonImage = pushButton.GetComponent<Image>(); // Get the button's image
            originalButtonColor = buttonImage.color; // Store original color

            // Find or create the timer text inside the button
            pushButtonText = pushButton.GetComponentInChildren<TextMeshProUGUI>();
            if (pushButtonText != null)
            {
                pushButtonText.text = "FART"; // Default text
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner || joystick == null || !canMove) return;

        Vector2 moveInput = joystick.GetInputVector();
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);

        Vector3 desiredVelocity = movementDirection * speed;
        Vector3 force = (desiredVelocity - rb.linearVelocity) * acceleration;
        rb.AddForce(force, ForceMode.Acceleration);

        if (movementDirection.sqrMagnitude > 0.01f && IsGrounded())
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 10f));
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }

        animator.SetBool("Run", movementDirection.sqrMagnitude > 0.01f);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            if (gameOverText != null) gameOverText.gameObject.SetActive(true);
            rb.linearVelocity = Vector3.zero;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (SpawnManager.Instance == null)
            {
                Debug.LogError("SpawnManager.Instance is null!");
                return;
            }

            Transform spawnPoint = SpawnManager.Instance.GetSpawnPoint();
            if (spawnPoint == null)
            {
                Debug.LogError("No spawn point available!");
                return;
            }

            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation; // Set rotation as well
        }
    }

    private void AttemptPush()
    {
        if (IsOwner && !isPushing)
        {
            isPushing = true;
            pushButton.interactable = false; // Disable button
            buttonImage.color = Color.gray; // Change color to darker
            StartCoroutine(PushCooldown()); // Start cooldown timer
            PushPlayersServerRpc();
        }
    }

    [ServerRpc]
    private void PushPlayersServerRpc(ServerRpcParams rpcParams = default)
    {
        if (!IsServer) return;

        Debug.Log($"[Server] PushPlayersServerRpc called by ClientID: {rpcParams.Receive.SenderClientId}");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pushRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player") && hit.gameObject != gameObject)
            {
                Rigidbody otherRb = hit.GetComponent<Rigidbody>();
                if (otherRb != null)
                {
                    Vector3 pushDirection = (hit.transform.position - transform.position).normalized;
                    float pushForceApplied = pushForce;

                    Debug.Log($"[Server] Pushing player: {hit.gameObject.name}");
                    ApplyPushClientRpc(hit.GetComponent<NetworkObject>(), pushDirection * pushForceApplied);
                }
            }
        }
    }

    [ClientRpc]
    private void ApplyPushClientRpc(NetworkObjectReference playerRef, Vector3 force)
    {
        if (playerRef.TryGet(out NetworkObject networkObject))
        {
            Rigidbody rb = networkObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(force, ForceMode.Impulse);
                Debug.Log($"[Client] Applied force to {networkObject.gameObject.name}");
            }
        }
    }

    private System.Collections.IEnumerator PushCooldown()
    {
        float timer = pushCooldown;
        while (timer > 0)
        {
            if (pushButtonText != null)
                pushButtonText.text = timer.ToString("F1"); // Display countdown
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }

        // Reset button state after cooldown
        if (pushButtonText != null)
            pushButtonText.text = "FART";

        pushButton.interactable = true;
        buttonImage.color = originalButtonColor; // Restore original color
        isPushing = false;
    }
}
