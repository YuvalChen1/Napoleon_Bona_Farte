using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0, 5, -5);
    public float smoothSpeed = 0.125f;

    private void Start()
    {
        FindLocalPlayer();
    }

    private void LateUpdate()
    {
        if (player == null)
        {
            FindLocalPlayer(); // Try to find the player again if lost
            return;
        }

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    private void FindLocalPlayer()
    {
        foreach (var obj in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
        {
            if (obj.IsOwner) // Only assign the camera to the local player
            {
                player = obj.transform;
                Debug.Log($"Camera now follows: {player.name}");
                return;
            }
        }
    }
}
