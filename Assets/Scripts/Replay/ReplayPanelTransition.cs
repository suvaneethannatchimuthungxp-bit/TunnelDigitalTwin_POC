using System.Collections;
using UnityEngine;

public class ReplayPanelTransition : MonoBehaviour
{
    [Header("Panels")]
    public GameObject replayPanel;

    public GameObject eventPanel;

    [Header("Animation")]
    public CanvasGroup replayCanvas;

    public RectTransform replayRect;

    [Header("Animation Settings")]
    public float animationDuration = 0.35f;

    public Vector2 hiddenPosition =
        new Vector2(0, -250);

    public Vector2 visiblePosition =
        Vector2.zero;

    private bool isAnimating;

    // ==================================================
    // OPEN REPLAY PANEL
    // ==================================================

    public void OpenReplayPanel()
    {
        if (isAnimating)
            return;

        StartCoroutine(
            AnimateReplayOpen()
        );
    }

    IEnumerator AnimateReplayOpen()
    {
        isAnimating = true;

        // HIDE EVENT PANEL
        eventPanel.SetActive(false);

        // SHOW REPLAY PANEL
        replayPanel.SetActive(true);

        float timer = 0f;

        replayCanvas.alpha = 0f;

        replayRect.anchoredPosition =
            hiddenPosition;

        while (timer < animationDuration)
        {
            float t =
                timer / animationDuration;

            // SMOOTH EASING
            t = Mathf.SmoothStep(0, 1, t);

            // FADE
            replayCanvas.alpha =
                Mathf.Lerp(0f, 1f, t);

            // SLIDE
            replayRect.anchoredPosition =
                Vector2.Lerp(
                    hiddenPosition,
                    visiblePosition,
                    t
                );

            timer += Time.deltaTime;

            yield return null;
        }

        replayCanvas.alpha = 1f;

        replayRect.anchoredPosition =
            visiblePosition;

        isAnimating = false;
    }

    // ==================================================
    // CLOSE REPLAY PANEL
    // ==================================================

    public void CloseReplayPanel()
    {
        if (isAnimating)
            return;

        StartCoroutine(
            AnimateReplayClose()
        );
    }

    IEnumerator AnimateReplayClose()
    {
        isAnimating = true;

        float timer = 0f;

        while (timer < animationDuration)
        {
            float t =
                timer / animationDuration;

            t = Mathf.SmoothStep(0, 1, t);

            // FADE OUT
            replayCanvas.alpha =
                Mathf.Lerp(1f, 0f, t);

            // SLIDE DOWN
            replayRect.anchoredPosition =
                Vector2.Lerp(
                    visiblePosition,
                    hiddenPosition,
                    t
                );

            timer += Time.deltaTime;

            yield return null;
        }

        replayCanvas.alpha = 0f;

        replayRect.anchoredPosition =
            hiddenPosition;

        replayPanel.SetActive(false);

        // SHOW EVENT PANEL AGAIN
        eventPanel.SetActive(true);

        isAnimating = false;
    }
}