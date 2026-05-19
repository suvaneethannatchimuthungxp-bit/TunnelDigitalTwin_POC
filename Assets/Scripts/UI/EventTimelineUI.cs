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
        Color color,
        int replayIndex)
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

     //   Button replayButton =
     //obj.transform
     //.Find("ReplayIncidentButton")
     //.GetComponent<Button>();

        // MESSAGE
        texts[0].text =
      "<b>" + message + "</b>";

        // TIME
        texts[1].text =
      "RECORDED : " +
      System.DateTime.Now
      .ToString("hh:mm:ss tt");

        // ICON COLOR
        icon.color = color;


        //replayButton.onClick.AddListener(() =>
        //{
        //    ReplayReplayEvent(replayIndex);
        //});

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

    //void ReplayReplayEvent(int index)
    //{
    //    if (index < 0 ||
    //        index >= ReplayManager.Instance.replayEvents.Count)
    //    {
    //        return;
    //    }

    //    ReplayEvent replayEvent =
    //        ReplayManager.Instance.replayEvents[index];

       
    //    GameObject workerObj =
    //GameObject.Find(replayEvent.workerName);

    //    if (workerObj != null)
    //    {
    //        StartCoroutine(
    //            PlayReplayMovement(
    //                workerObj.transform,
    //                replayEvent
    //            )
    //        );
    //    }
    //    Debug.Log(
    //        "REPLAYING INCIDENT : " +
    //        replayEvent.eventMessage
    //    );

    //    Debug.Log(
    //        "WORKER : " +
    //        replayEvent.workerName
    //    );

    //    Debug.Log(
    //        "POSITION : " +
    //        replayEvent.workerPosition
    //    );
    //}

    //System.Collections.IEnumerator PlayReplayMovement(
    //Transform worker,
    //ReplayEvent replayEvent)
    //{
    //    ReplayHighlighter highlighter =
    //worker.GetComponent<ReplayHighlighter>();

        //if (highlighter != null)
        //{
        //    highlighter.HighlightReplay();
        //}
        //if (replayEvent.recordedPositions.Count == 0)
        //{
        //    yield break;
        //}

    //    for (int i = 0;
    //         i < replayEvent.recordedPositions.Count;
    //         i++)
    //    {
    //        worker.position =
    //            replayEvent.recordedPositions[i];

    //        yield return new WaitForSeconds(0.08f);
    //    }
    //}
}