using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReplayTimelineUI : MonoBehaviour
{
    [Header("UI")]
    public Slider timelineSlider;

    public TMP_Text timeText;

    public TMP_Text modeText;

    [Header("Replay")]
    public GlobalReplayManager replayManager;

    private bool isDragging;

    void Start()
    {
        timelineSlider.minValue = 0;
    }

    void Update()
    {
        if (replayManager == null)
            return;

        if (replayManager.IsReplayPlaying())
        {
            timelineSlider.SetValueWithoutNotify(
                replayManager.currentReplayFrame
            );
        }
        // UPDATE MAX FRAMES
        timelineSlider.maxValue =
            replayManager.replayFrames.Count - 1;

        // LIVE MODE
        if (!replayManager.IsReplayPlaying())
        {
            modeText.text =
                "● LIVE MODE";

            modeText.color =
                Color.green;
        }
        else
        {
            modeText.text =
                "● REPLAY MODE";

            modeText.color =
                Color.cyan;
        }

        // UPDATE TIME TEXT
        int frame =
            Mathf.RoundToInt(
                timelineSlider.value
            );

        timeText.text =
      "FRAME : " +
      replayManager.currentReplayFrame;
    }

    // =========================================
    // SCRUB TIMELINE
    // =========================================

    public void OnSliderChanged()
    {
        if (replayManager == null)
            return;

        // DON'T SCRUB DURING PLAYBACK
        if (replayManager.IsReplayPlaying())
            return;

        int frame =
            Mathf.RoundToInt(
                timelineSlider.value
            );

        replayManager.currentReplayFrame =
            frame;

        replayManager.RestoreFrame(frame);
    }
    // =========================================
    // PLAY BUTTON
    // =========================================

    public void PlayReplay()
    {
        replayManager.PlayReplayFromUI();
    }

    // =========================================
    // STOP BUTTON
    // =========================================

    public void StopReplay()
    {
        replayManager.StopReplay();
    }

    public void PauseReplay()
    {
        replayManager.PauseReplay();
    }
}