using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainLobbyPanel;
    public GameObject privateMatchPanel;

    [Header("Buttons")]
    public Button playButton;
    public Button privateMatchButton;
    public Button backButton;

    [Header("Queue UI")]
    public GameObject queueLoaderPanel;


    void Start()
    {
        // Show main lobby at start
        mainLobbyPanel.SetActive(true);
        privateMatchPanel.SetActive(false);

        // Hook up button events
        playButton.onClick.AddListener(OnPlayClicked);
        privateMatchButton.onClick.AddListener(OnPrivateMatchClicked);
        backButton.onClick.AddListener(OnBackClicked);
    }

    void OnPlayClicked()
    {
        Debug.Log("Play clicked (matchmaking)");

        // If we are already in a network session, just join the queue
        if (NetworkManager.Singleton.IsHost)
        {
            // Host starts the session
            Debug.Log("Starting host...");
            NetworkManager.Singleton.StartHost();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            // Client just joins the session
            Debug.Log("Joining as client...");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            // If no session is active, try starting the session as a host (for Player 1)
            Debug.Log("No session found, starting as host...");
            NetworkManager.Singleton.StartHost();
        }

        ShowQueueLoader(); // Show visual feedback for searching players

        MatchmakingManager.Instance.JoinQueue(); // Add player to the queue
    }

    void OnPrivateMatchClicked()
    {
        mainLobbyPanel.SetActive(false);
        privateMatchPanel.SetActive(true);
    }

    void OnBackClicked()
    {
        privateMatchPanel.SetActive(false);
        mainLobbyPanel.SetActive(true);
    }

    public void ShowQueueLoader()
    {
        if (queueLoaderPanel != null)
        {
            queueLoaderPanel.SetActive(true);
        }
    }
}
