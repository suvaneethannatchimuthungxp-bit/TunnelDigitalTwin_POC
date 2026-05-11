using UnityEngine;
using UnityEngine.UI;

public class ViewControlUI : MonoBehaviour
{
    [Header("Toggles")]
    public Toggle zonesToggle;

    public Toggle workersToggle;

    public Toggle tunnelToggle;


    [Header("Objects")]
    public GameObject zonesRoot;

    public GameObject workersRoot;

    public GameObject tunnelRoot;


    // ==================================================
    // START
    // ==================================================

    void Start()
    {
        // INITIAL STATES
        zonesToggle.isOn = true;
        workersToggle.isOn = true;
        tunnelToggle.isOn = true;

        // LISTENERS
        zonesToggle.onValueChanged
            .AddListener(ToggleZones);

        workersToggle.onValueChanged
            .AddListener(ToggleWorkers);

        tunnelToggle.onValueChanged
            .AddListener(ToggleLabels);

      
    }

    // ==================================================
    // ZONES
    // ==================================================

    void ToggleZones(bool state)
    {
        zonesRoot.SetActive(state);
    }

    // ==================================================
    // WORKERS
    // ==================================================

    void ToggleWorkers(bool state)
    {
        workersRoot.SetActive(state);
    }

    // ==================================================
    // LABELS
    // ==================================================

    void ToggleLabels(bool state)
    {
        tunnelRoot.SetActive(state);
    }

  
}