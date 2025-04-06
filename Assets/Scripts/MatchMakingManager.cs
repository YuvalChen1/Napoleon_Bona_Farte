using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MatchmakingManager : MonoBehaviour
{
    public static MatchmakingManager Instance;
    private List<ulong> queue = new List<ulong>();
    public int requiredPlayers = 2;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void JoinQueue()
    {
        if (!NetworkManager.Singleton.IsConnectedClient)
        {
            Debug.LogError("Client is not connected.");
            return;
        }

        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Host started the match.");
            AddPlayerToQueue(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            Debug.Log("Client trying to join the queue...");
            JoinQueueServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void JoinQueueServerRpc(ServerRpcParams rpcParams = default)
    {
        AddPlayerToQueue(rpcParams.Receive.SenderClientId);
    }

    private void AddPlayerToQueue(ulong clientId)
    {
        if (!queue.Contains(clientId))
        {
            queue.Add(clientId);
            Debug.Log($"Player {clientId} joined queue.");

            if (queue.Count >= requiredPlayers)
            {
                StartMatch();
            }
        }
    }

    private void StartMatch()
    {
        Debug.Log("Match ready. Starting...");

        // Load the GameScene for all players
        foreach (ulong clientId in queue)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }

        queue.Clear(); // Reset the queue after match start
    }
}
