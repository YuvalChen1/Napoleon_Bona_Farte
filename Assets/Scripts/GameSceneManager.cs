using TMPro;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public TextMeshProUGUI joinCodeText; // Reference to the UI Text component in the GameScene

    void Start()
    {
        // Retrieve the join code from PlayerPrefs and display it
        string joinCode = PlayerPrefs.GetString("JoinCode", "No Code Available");
        joinCodeText.text = "Join Code: " + joinCode;
    }
}
