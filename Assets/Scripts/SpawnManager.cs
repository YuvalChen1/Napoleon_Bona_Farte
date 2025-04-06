using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    public static SpawnManager Instance;

    public Transform[] spawnPoints; // Assign in Inspector

    private List<int> usedSpawnIndices = new List<int>();

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
            Debug.Log("SpawnManager instance created and set to persist.");
        }
        else
        {
            Debug.LogWarning("Another instance of SpawnManager exists, destroying it.");
            Destroy(gameObject); // Destroy if another instance exists
        }
    }

    public Transform GetSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return null;
        }

        List<int> availableIndices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedSpawnIndices.Contains(i))
                availableIndices.Add(i);
        }

        if (availableIndices.Count == 0)
        {
            Debug.LogWarning("All spawn points used. Reusing first.");
            return spawnPoints[0];
        }

        int randomIndex = availableIndices[Random.Range(0, availableIndices.Count)];
        usedSpawnIndices.Add(randomIndex);

        return spawnPoints[randomIndex];
    }
}
