using System.Collections;
using UnityEngine;

public class RightPanelUI : MonoBehaviour
{
    [Header("Panel")]
    public RectTransform panel;

    public CanvasGroup canvasGroup;

    [Header("Animation")]
    public float animationDuration = 0.4f;

    public AnimationCurve animationCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Positions")]
    public Vector2 shownPosition;

    public Vector2 hiddenPosition;

    Coroutine currentRoutine;

    // ==================================================
    // SHOW PANEL
    // ==================================================

    public void ShowPanel()
    {
        panel.gameObject.SetActive(true);

        gameObject.SetActive(true);

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine =
            StartCoroutine(
                AnimatePanel(
                    hiddenPosition,
                    shownPosition,
                    0f,
                    1f
                )
            );
    }

    // ==================================================
    // HIDE PANEL
    // ==================================================

    public void HidePanel()
    {
        if (!gameObject.activeSelf)
            return;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine =
            StartCoroutine(
                AnimatePanel(
                    shownPosition,
                    hiddenPosition,
                    1f,
                    0f,
                    true
                )
            );
    }

    // ==================================================
    // ANIMATION
    // ==================================================

    IEnumerator AnimatePanel(
        Vector2 startPos,
        Vector2 endPos,
        float startAlpha,
        float endAlpha,
        bool disableAfter = false
    )
    {
        float time = 0;

        while (time < animationDuration)
        {
            time += Time.deltaTime;

            float t =
                animationCurve.Evaluate(
                    time / animationDuration
                );

            // POSITION
            panel.anchoredPosition =
                Vector2.Lerp(
                    startPos,
                    endPos,
                    t
                );

            // ALPHA
            canvasGroup.alpha =
                Mathf.Lerp(
                    startAlpha,
                    endAlpha,
                    t
                );

            yield return null;
        }

        panel.anchoredPosition =
            endPos;

        canvasGroup.alpha =
            endAlpha;

        if (disableAfter)
        {
            panel.gameObject.SetActive(false);
        }
    }
}