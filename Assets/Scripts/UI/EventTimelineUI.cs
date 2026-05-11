using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventTimelineUI : MonoBehaviour
{
    [Header("UI")]
    public Transform contentParent;

    public GameObject eventPrefab;

    [Header("Limit")]
    public int maxEvents = 10;

    // ==================================================
    // ADD EVENT
    // ==================================================

    public void AddEvent(
        string message,
        Color color)
    {
        GameObject obj =
            Instantiate(
                eventPrefab,
                contentParent
            );

        // UI References
        TMP_Text[] texts =
            obj.GetComponentsInChildren<TMP_Text>();

        Image icon =
            obj.GetComponentInChildren<Image>();

        // MESSAGE
        texts[0].text = message;

        // TIME
        texts[1].text =
            System.DateTime.Now
            .ToString("hh:mm:ss tt");

        // ICON COLOR
        icon.color = color;

        // LIMIT EVENTS
        if (contentParent.childCount > maxEvents)
        {
            Destroy(
                contentParent
                .GetChild(0)
                .gameObject
            );
        }
    }
}