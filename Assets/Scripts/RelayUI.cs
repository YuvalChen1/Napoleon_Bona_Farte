using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;
using System.Text.RegularExpressions;

public class RelayUI : MonoBehaviour
{
    public RelayManager relayManager;
    public Button createRelayButton;
    public Button joinRelayButton;
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay;
    public TMP_Text errorMessageText; // Text for error messages
    public GameObject relayPanel;
    private bool isJoining;

    private void Start()
    {
        createRelayButton.onClick.AddListener(() => StartCoroutine(CreateRelaySession()));
        joinRelayButton.onClick.AddListener(() => StartCoroutine(JoinRelaySession()));

        errorMessageText.gameObject.SetActive(false); // Hide error message initially
    }

    private IEnumerator CreateRelaySession()
    {
        Task<string> createRelayTask = relayManager.CreateRelay(4);
        yield return new WaitUntil(() => createRelayTask.IsCompleted);

        string joinCode = createRelayTask.Result;
        if (!string.IsNullOrEmpty(joinCode))
        {
            joinCodeDisplay.text = "Join Code: " + joinCode;
            GUIUtility.systemCopyBuffer = joinCode;
            relayPanel.SetActive(false);
        }
    }

    private IEnumerator JoinRelaySession()
    {
        string joinCode = joinCodeInput.text;

        // Validate the join code format before attempting to join
        if (IsJoinCodeValid(joinCode))
        {
            // Disable the UI or show loading state if needed
            isJoining = true;
            errorMessageText.gameObject.SetActive(false); // Hide error message initially

            // Call JoinRelay and wait for it to complete
            Task joinRelayTask = relayManager.JoinRelay(joinCode);
            yield return new WaitUntil(() => joinRelayTask.IsCompleted);

            // Stop waiting and handle errors
            isJoining = false;

            if (joinRelayTask.IsFaulted)
            {
                // If the task failed, check if it's the "Room not found" error
                if (joinRelayTask.Exception != null && joinRelayTask.Exception.InnerException != null &&
                    joinRelayTask.Exception.InnerException.Message == "Room not found")
                {
                    errorMessageText.text = "Room not found!";
                }
                else
                {
                    errorMessageText.text = "Connection failed. Please try again.";
                }

                errorMessageText.gameObject.SetActive(true); // Show error message
            }
            else
            {
                // If the join was successful, hide the relay panel
                relayPanel.SetActive(false);
                errorMessageText.gameObject.SetActive(false); // Hide any previous error
            }
        }
        else
        {
            // Show an error if the code format is invalid
            errorMessageText.text = "Invalid join code!";
            errorMessageText.gameObject.SetActive(true);
        }
    }

    // Validate the join code format
    private bool IsJoinCodeValid(string joinCode)
    {
        string pattern = "^[6789BCDFGHJKLMNPQRTWbcdfghjklmnpqrtw]{6,12}$"; // Valid join code pattern
        return Regex.IsMatch(joinCode, pattern);
    }
}
