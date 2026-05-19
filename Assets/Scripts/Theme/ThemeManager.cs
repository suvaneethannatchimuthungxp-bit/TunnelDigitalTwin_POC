using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ThemeManager : MonoBehaviour
{
    [Header("PANELS")]
    public List<Image> panels;

    [Header("TEXTS")]
    public List<TextMeshProUGUI> texts;

    [Header("BACKGROUNDS")]
    public List<Image> backgrounds;

    [Header("TOGGLE UI")]
    public RectTransform toggleCircle;

    public Image themeIcon;

    public Sprite daySprite;

    public Sprite nightSprite;

    public Vector2 darkPosition;

    public Vector2 lightPosition;

    [Header("TOGGLE TEXT")]
    public TextMeshProUGUI toggleText;

    [Header("TRANSITION SETTINGS")]
    [Range(0.5f, 5f)]
    public float transitionSpeed = 2f;

    private bool isDark = true;

    // ======================================================
    // PREMIUM DARK MODE
    // ======================================================

    Color darkPanel =
        new Color32(18, 28, 40, 240);

    Color darkText =
        new Color32(235, 242, 248, 255);

    Color darkBackground =
        new Color32(10, 18, 28, 255);

    // ======================================================
    // PREMIUM LIGHT MODE
    // ======================================================

    Color lightPanel =
      new Color32(248, 249, 251, 255);

    Color lightText =
        new Color32(25, 35, 48, 255);

    Color lightBackground =
        new Color32(228, 233, 238, 255);
    // ======================================================
    // START
    // ======================================================

    void Start()
    {
        ApplyThemeInstant();

        UpdateToggleInstant();
    }

    // ======================================================
    // TOGGLE THEME
    // ======================================================

    public void ToggleTheme()
    {
        isDark = !isDark;

        StopAllCoroutines();

        StartCoroutine(SmoothThemeTransition());

        StartCoroutine(AnimateToggle());

        if (toggleText != null)
        {
            toggleText.text =
                isDark ? "Night" : "Day";
        }
    }

    // ======================================================
    // UPDATE TOGGLE UI
    // ======================================================

    void UpdateToggleInstant()
    {
        if (toggleCircle != null)
        {
            toggleCircle.anchoredPosition =
                isDark ?
                darkPosition :
                lightPosition;
        }

        if (themeIcon != null)
        {
            themeIcon.sprite =
                isDark ?
                nightSprite :
                daySprite;
        }
    }

    // ======================================================
    // APPLY THEME INSTANT
    // ======================================================

    void ApplyThemeInstant()
    {
        Color panelColor =
            isDark ?
            darkPanel :
            lightPanel;

        Color textColor =
            isDark ?
            darkText :
            lightText;

        Color bgColor =
            isDark ?
            darkBackground :
            lightBackground;

        // ======================================================
        // PANELS
        // ======================================================

        foreach (Image img in panels)
        {
            if (img != null)
            {
                img.color = panelColor;
            }
        }

        // ======================================================
        // TEXTS
        // ======================================================

        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt != null)
            {
                txt.color = textColor;
            }
        }

        // ======================================================
        // BACKGROUNDS
        // ======================================================

        foreach (Image img in backgrounds)
        {
            if (img != null)
            {
                img.color = bgColor;
            }
        }

        // ======================================================
        // TOGGLE TEXT
        // ======================================================

        if (toggleText != null)
        {
            toggleText.text =
                isDark ? "Day" : "Night";
        }
    }

    // ======================================================
    // SMOOTH TRANSITION
    // ======================================================

    IEnumerator SmoothThemeTransition()
    {
        float time = 0f;

        List<Color> panelStart =
            new List<Color>();

        List<Color> textStart =
            new List<Color>();

        List<Color> bgStart =
            new List<Color>();

        // STORE START COLORS

        foreach (Image img in panels)
        {
            if (img != null)
            {
                panelStart.Add(img.color);
            }
        }

        foreach (TextMeshProUGUI txt in texts)
        {
            if (txt != null)
            {
                textStart.Add(txt.color);
            }
        }

        foreach (Image img in backgrounds)
        {
            if (img != null)
            {
                bgStart.Add(img.color);
            }
        }

        // TARGET COLORS

        Color targetPanel =
            isDark ?
            darkPanel :
            lightPanel;

        Color targetText =
            isDark ?
            darkText :
            lightText;

        Color targetBG =
            isDark ?
            darkBackground :
            lightBackground;

        // TRANSITION LOOP

        while (time < 1f)
        {
            time +=
                Time.deltaTime *
                transitionSpeed;

            // ======================================================
            // PANELS
            // ======================================================

            for (int i = 0; i < panels.Count; i++)
            {
                if (panels[i] != null)
                {
                    panels[i].color =
                        Color.Lerp(
                            panelStart[i],
                            targetPanel,
                            time
                        );
                }
            }

            // ======================================================
            // TEXTS
            // ======================================================

            for (int i = 0; i < texts.Count; i++)
            {
                if (texts[i] != null)
                {
                    texts[i].color =
                        Color.Lerp(
                            textStart[i],
                            targetText,
                            time
                        );
                }
            }

            // ======================================================
            // BACKGROUNDS
            // ======================================================

            for (int i = 0; i < backgrounds.Count; i++)
            {
                if (backgrounds[i] != null)
                {
                    backgrounds[i].color =
                        Color.Lerp(
                            bgStart[i],
                            targetBG,
                            time
                        );
                }
            }

            yield return null;
        }
    }

    // ======================================================
    // TOGGLE ANIMATION
    // ======================================================

    IEnumerator AnimateToggle()
    {
        Vector2 startPos =
            toggleCircle.anchoredPosition;

        Vector2 targetPos =
            isDark ?
            darkPosition :
            lightPosition;

        Sprite targetSprite =
            isDark ?
            nightSprite :
            daySprite;

        if (themeIcon != null)
        {
            themeIcon.sprite =
                targetSprite;
        }

        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * 6f;

            toggleCircle.anchoredPosition =
                Vector2.Lerp(
                    startPos,
                    targetPos,
                    t
                );

            // PREMIUM SOFT SCALE EFFECT

            float scale =
                1 +
                Mathf.Sin(
                    t * Mathf.PI
                ) * 0.05f;

            toggleCircle.localScale =
                Vector3.one * scale;

            yield return null;
        }

        toggleCircle.localScale =
            Vector3.one;
    }
}