using UnityEngine;
using UnityEngine.UI;

public class CameraUIManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject cameraPanel;

    [Header("Camera")]
    public DigitalTwinCamera digitalTwinCamera;

    [Header("Buttons")]
    public Button topViewButton;

    public Button cctvButton;

    public Button autoViewButton;

    public Button freeCamButton;

    [Header("Button Colors")]
    public Color normalColor =
        Color.white;

    public Color selectedColor =
        Color.cyan;

    private void Start()
    {
        HideCameraPanel();
        FreeCam();
    }
    // =================================================
    // OPEN CAMERA PANEL
    // =================================================

    public void OpenCameraPanel()
    {
        cameraPanel.SetActive(true);
    }

    // =================================================
    // HIDE CAMERA PANEL
    // =================================================

    public void HideCameraPanel()
    {
        cameraPanel.SetActive(false);
    }

    // =================================================
    // TOP VIEW
    // =================================================

    public void TopView()
    {
        digitalTwinCamera.TopViewMode();

        SelectTopView();

       
    }

    // =================================================
    // CCTV VIEW
    // =================================================

    public void CCTVView()
    {
        digitalTwinCamera.CCTVViewMode();

        SelectCCTVView();

      
    }

    // =================================================
    // AUTO VIEW
    // =================================================

    public void AutoView()
    {
        digitalTwinCamera.AutoLookMode();

        SelectAutoView();

    }

    // =================================================
    // FREE CAM
    // =================================================

    public void FreeCam()
    {
        digitalTwinCamera.FreeCamMode();

        SelectFreeCam();

       
    }

    // =================================================
    // SELECT TOP VIEW
    // =================================================

    void SelectTopView()
    {
        ResetAllButtons();

        SetButtonColor(
            topViewButton,
            selectedColor
        );
    }

    // =================================================
    // SELECT CCTV
    // =================================================

    void SelectCCTVView()
    {
        ResetAllButtons();

        SetButtonColor(
            cctvButton,
            selectedColor
        );
    }

    // =================================================
    // SELECT AUTO VIEW
    // =================================================

    void SelectAutoView()
    {
        ResetAllButtons();

        SetButtonColor(
            autoViewButton,
            selectedColor
        );
    }

    // =================================================
    // SELECT FREE CAM
    // =================================================

   public void SelectFreeCam()
    {
        ResetAllButtons();

        SetButtonColor(
            freeCamButton,
            selectedColor
        );
    }

    // =================================================
    // RESET BUTTONS
    // =================================================

    void ResetAllButtons()
    {
        SetButtonColor(
            topViewButton,
            normalColor
        );

        SetButtonColor(
            cctvButton,
            normalColor
        );

        SetButtonColor(
            autoViewButton,
            normalColor
        );

        SetButtonColor(
            freeCamButton,
            normalColor
        );
    }

    // =================================================
    // BUTTON COLOR
    // =================================================

    void SetButtonColor(
        Button button,
        Color color
    )
    {
        ColorBlock cb =
            button.colors;

        cb.normalColor = color;
        cb.selectedColor = color;
        cb.highlightedColor = color;

        button.colors = cb;
    }
}