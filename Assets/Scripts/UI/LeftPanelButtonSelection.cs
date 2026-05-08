using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeftPanelButtonSelection : MonoBehaviour
{
    [Header("Sidebar Buttons")]
    public Button[] buttons;

    [Header("Button Colors")]
    public Color normalButtonColor =
        new Color(0.05f, 0.10f, 0.16f);

    public Color selectedButtonColor =
        new Color(0.06f, 0.38f, 1f);

    [Header("Text Colors")]
    public Color normalTextColor =
        new Color(0.75f, 0.78f, 0.82f);

    public Color selectedTextColor =
        Color.white;

    void Start()
    {
        // DEFAULT SELECT FIRST BUTTON
        if (buttons.Length > 0)
        {
            SelectButton(buttons[0]);
        }
    }

    // ==================================================
    // SELECT BUTTON
    // ==================================================

    public void SelectButton(Button selectedButton)
    {
        // RESET ALL BUTTONS
        foreach (Button btn in buttons)
        {
            if (btn == null)
                continue;

            // BACKGROUND
            Image bg =
                btn.GetComponent<Image>();

            if (bg != null)
            {
                bg.color =
                    normalButtonColor;
            }

            // TEXT
            TMP_Text txt =
                btn.GetComponentInChildren<TMP_Text>();

            if (txt != null)
            {
                txt.color =
                    normalTextColor;
            }
        }

        // APPLY SELECTED STYLE
        if (selectedButton != null)
        {
            Image selectedBG =
                selectedButton.GetComponent<Image>();

            if (selectedBG != null)
            {
                selectedBG.color =
                    selectedButtonColor;
            }

            TMP_Text selectedTXT =
                selectedButton.GetComponentInChildren<TMP_Text>();

            if (selectedTXT != null)
            {
                selectedTXT.color =
                    selectedTextColor;
            }
        }
    }
}