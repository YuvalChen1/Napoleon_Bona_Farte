using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;        // Reference to the player's transform
    public Vector3 offset;          // Offset distance between the player and camera
    public float smoothSpeed = 0.125f; // Smoothing factor

    private void LateUpdate()
    {
        // Check if the player is assigned
        if (player != null)
        {
            // Get the desired position of the camera
            Vector3 desiredPosition = player.position + offset;

            // Smoothly move the camera to the new position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;

            // Keep the camera's rotation fixed to the initial value (no tilt)
            transform.rotation = Quaternion.Euler(45, 0, 0);
        }
        else
        {
            // Optionally, log a warning if the player is missing
            Debug.LogWarning("Player reference is missing in CameraFollow script.");
        }
    }
}
