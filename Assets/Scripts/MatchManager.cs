using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;

public class MatchManager : NetworkBehaviour
{
    public static MatchManager Instance;

    public TextMeshProUGUI countdownText;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client {clientId} connected. Current player count: {NetworkManager.Singleton.ConnectedClients.Count}");

        // Wait until at least 2 players are connected
        if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
        {
            StartCoroutine(StartMatchCountdown());
        }
    }

    private IEnumerator StartMatchCountdown()
    {
        yield return new WaitForSeconds(1f); // Small delay before countdown

        for (int i = 3; i > 0; i--)
        {
            UpdateCountdownClientRpc(i.ToString());
            yield return new WaitForSeconds(1f);
        }

        UpdateCountdownClientRpc("Go!");
        yield return new WaitForSeconds(1f);
        UpdateCountdownClientRpc("");

        // Optionally enable movement here if it was disabled
        EnablePlayerMovementClientRpc();
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(string message)
    {
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = message;
        }
    }

    [ClientRpc]
    private void EnablePlayerMovementClientRpc()
    {
        foreach (var player in FindObjectsOfType<PlayerMovement>())
        {
            if (player.IsOwner)
                player.canMove = true;
        }
    }
}
