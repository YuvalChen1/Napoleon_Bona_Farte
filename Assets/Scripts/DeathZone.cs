using UnityEngine;

public class DeathZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Ensure player GameObject has the "Player" tag
        {
            Destroy(other.gameObject); // Remove player from scene (you can replace with a death animation)
            Debug.Log("Player Fell Off!");
        }
    }
}
