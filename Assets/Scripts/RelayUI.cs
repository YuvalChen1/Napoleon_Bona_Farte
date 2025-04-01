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
        }
    }

    private IEnumerator JoinRelaySession()
    {
        string joinCode = joinCodeInput.text;
        if (!string.IsNullOrEmpty(joinCode))
        {
            Task joinRelayTask = relayManager.JoinRelay(joinCode);
            yield return new WaitUntil(() => joinRelayTask.IsCompleted);
        }
    }
}
