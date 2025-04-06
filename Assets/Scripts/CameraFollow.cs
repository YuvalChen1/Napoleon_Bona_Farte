using UnityEngine;
using Unity.Netcode;

public class CameraFollow : MonoBehaviour
{
    private Transform player;
    public Vector3 offset = new Vector3(0, 5, -5);
    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        if (player == null)
        {
            TryFindLocalPlayer();
            return;
        }

        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    private void TryFindLocalPlayer()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsConnectedClient)
            return;

        var ownedObjects = NetworkManager.Singleton.SpawnManager?.GetClientOwnedObjects(NetworkManager.Singleton.LocalClientId);
        if (ownedObjects == null) return;

        foreach (var obj in ownedObjects)
        {
            if (obj.TryGetComponent(out NetworkObject netObj))
            {
                player = obj.transform;
                Debug.Log($"[CameraFollow] Following local player: {player.name}");
                break;
            }
        }
    }

}
