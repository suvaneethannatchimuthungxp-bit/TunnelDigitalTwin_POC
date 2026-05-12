using UnityEngine;

public class HelpPanelUI : MonoBehaviour
{
    public GameObject helpPanel;


    void Start()
    {
        helpPanel.SetActive(false);
    }

    public void ShowHelpPanel()
    {
        helpPanel.SetActive(true);
    }

    public void HideHelpPanel()
    {
        helpPanel.SetActive(false);
    }
}