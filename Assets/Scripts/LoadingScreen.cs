using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    public Slider progressBar; // Reference to the UI Slider (progress bar)
    public TextMeshProUGUI progressText; // Optional: Display percentage

    void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("LobbyScene");
        operation.allowSceneActivation = false; // Prevent auto-switching to MainScene

        float simulatedProgress = 0f; // Start progress at 0

        while (simulatedProgress < 0.9f) // Simulate a progress bar until it's "almost full"
        {
            // Increase simulated progress slowly to simulate loading time
            simulatedProgress += 0.01f; // Adjust the speed here (0.01f = slow, 0.05f = faster)
            if (simulatedProgress > operation.progress) // Ensure it doesn't exceed the actual async loading progress
            {
                simulatedProgress = operation.progress;
            }

            // Update progress bar and text
            progressBar.value = simulatedProgress;
            progressText.text = Mathf.RoundToInt(simulatedProgress * 100) + "%";

            yield return new WaitForSeconds(0.05f); // Optional delay for slower progress (adjust this for faster/slower loading)
        }

        // Simulate the last 10% by slowly increasing progress from 90% to 100%
        while (simulatedProgress < 1f)
        {
            simulatedProgress += 0.01f; // Slow down the progress to fill the last 10%
            progressBar.value = simulatedProgress;
            progressText.text = Mathf.RoundToInt(simulatedProgress * 100) + "%";

            yield return new WaitForSeconds(0.05f); // Optional delay to simulate the loading time
        }

        // Ensure the progress bar reaches 100% before switching scenes
        progressBar.value = 1;
        progressText.text = "100%";
        yield return new WaitForSeconds(1); // Optional delay

        operation.allowSceneActivation = true; // Switch to MainScene
    }
}
