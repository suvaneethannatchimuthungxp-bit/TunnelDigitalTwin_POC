using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class HomeMenuLoader : MonoBehaviour
{
    [Header("Scene")]

    public string sceneToLoad =
        "MainTunnelScene";

    [Header("Loading UI")]

    public GameObject loadingPanel;

    public Image progressBarFill;

    public TMP_Text progressText;

    public TMP_Text statusText;

    [Header("Animation")]

    public CanvasGroup homeCanvasGroup;

    public float fadeSpeed = 2f;

    [Header("Status Messages")]

    public string[] loadingMessages =
    {
        "Initializing Digital Twin...",
        "Connecting IoT telemetry...",
        "Syncing environment systems...",
        "Loading worker tracking...",
        "Preparing visualization engine...",
        "Launching monitoring platform..."
    };

    bool isLoading = false;

    public void EnterMonitoring()
    {
        if (isLoading)
            return;

        StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        isLoading = true;

        // =========================
        // FADE HOME UI
        // =========================

        float fade = 1;

        while (fade > 0)
        {
            fade -= Time.deltaTime * fadeSpeed;

            homeCanvasGroup.alpha = fade;

            yield return null;
        }

        homeCanvasGroup.alpha = 0;

        // =========================
        // SHOW LOADING PANEL
        // =========================

        loadingPanel.SetActive(true);

        progressBarFill.fillAmount = 0;

        progressText.text = "0%";

        // =========================
        // START ASYNC LOAD
        // =========================

        AsyncOperation asyncLoad =
            SceneManager.LoadSceneAsync(sceneToLoad);

        asyncLoad.allowSceneActivation = false;

        float fakeProgress = 0;

        int messageIndex = 0;

        while (fakeProgress < 1f)
        {
            fakeProgress += Time.deltaTime * 0.25f;

            progressBarFill.fillAmount =
                fakeProgress;

            int percent =
                Mathf.RoundToInt(fakeProgress * 100);

            progressText.text =
                percent + "%";

            // CHANGE STATUS MESSAGE

            int newMessageIndex =
                Mathf.FloorToInt(fakeProgress * loadingMessages.Length);

            newMessageIndex =
                Mathf.Clamp(
                    newMessageIndex,
                    0,
                    loadingMessages.Length - 1);

            if (newMessageIndex != messageIndex)
            {
                messageIndex = newMessageIndex;

                statusText.text =
                    loadingMessages[messageIndex];
            }

            yield return null;
        }

        // =========================
        // FINALIZE
        // =========================

        progressBarFill.fillAmount = 1;

        progressText.text = "100%";

        statusText.text =
            "Initialization Complete";

        yield return new WaitForSeconds(0.5f);

        asyncLoad.allowSceneActivation = true;
    }
}