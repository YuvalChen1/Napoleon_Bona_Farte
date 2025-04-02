using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;

public class RelayUI : MonoBehaviour
{
    public RelayManager relayManager; // Reference to RelayManager script
    public Button createRelayButton;
    public Button joinRelayButton;
    public TMP_InputField joinCodeInput;
    public TMP_Text joinCodeDisplay; // Displays the join code
    public GameObject relayPanel; // Assign the UI panel that should be hidden

    private void Start()
    {
        createRelayButton.onClick.AddListener(() => StartCoroutine(CreateRelaySession()));
        joinRelayButton.onClick.AddListener(() => StartCoroutine(JoinRelaySession()));
    }

    private IEnumerator CreateRelaySession()
    {
        Task<string> createRelayTask = relayManager.CreateRelay(4);
        yield return new WaitUntil(() => createRelayTask.IsCompleted);

        string joinCode = createRelayTask.Result;
        if (!string.IsNullOrEmpty(joinCode))
        {
            joinCodeDisplay.text = "Join Code: " + joinCode;
            GUIUtility.systemCopyBuffer = joinCode; // Copies join code to clipboard

            // Hide the Relay UI for the Host
            relayPanel.SetActive(false);
        }
    }

    private IEnumerator JoinRelaySession()
    {
        string joinCode = joinCodeInput.text;
        if (!string.IsNullOrEmpty(joinCode))
        {
            Task joinRelayTask = relayManager.JoinRelay(joinCode);
            yield return new WaitUntil(() => joinRelayTask.IsCompleted);

            // Hide the Relay UI for the Client who joined
            relayPanel.SetActive(false);
        }
    }
}
