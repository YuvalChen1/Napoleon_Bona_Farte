using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System;

public class RelayManager : MonoBehaviour
{
    private async void Start()
    {
        await InitializeUnityServices();
    }

    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async Task<string> CreateRelay(int maxConnections)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log($"Relay created! Join Code: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            // Register callback to start host after scene is loaded
            SceneManager.sceneLoaded += OnGameSceneLoadedAsHost;

            SceneManager.LoadScene("GameScene");

            return joinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError($"Relay create error: {e.Message}");
            return null;
        }
    }

    public async Task JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            Debug.Log($"Joined Relay with Code: {joinCode}");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            // Register callback to start client after scene is loaded
            SceneManager.sceneLoaded += OnGameSceneLoadedAsClient;

            SceneManager.LoadScene("GameScene");
        }
        catch (RelayServiceException e)
        {
            if (e.Message.Contains("Not Found"))
            {
                Debug.LogError($"Join Relay failed: Room with join code '{joinCode}' not found.");
                throw new Exception("Room not found");
            }
            else
            {
                Debug.LogError($"Relay join error: {e.Message}");
                throw;
            }
        }
    }

    // Called once GameScene is fully loaded (for host)
    private void OnGameSceneLoadedAsHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoadedAsHost;
            NetworkManager.Singleton.StartHost();
        }
    }

    // Called once GameScene is fully loaded (for client)
    private void OnGameSceneLoadedAsClient(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            SceneManager.sceneLoaded -= OnGameSceneLoadedAsClient;
            NetworkManager.Singleton.StartClient();
        }
    }
}
