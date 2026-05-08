using UnityEngine;
using TMPro;
using System;

public class TopPanelUI : MonoBehaviour
{
    [Header("Time UI")]
    public TMP_Text timeText;

    public TMP_Text dateText;

    [Header("Live UI")]
    public GameObject liveDot;

    float blinkTimer;

    bool isVisible = true;

    [Header("Worker Stats")]
    public WorkerStatus[] workers;

    public TMP_Text totalWorkersText;

    public TMP_Text activeWorkersText;

    public TMP_Text idleWorkersText;

    public TMP_Text alertsText;

    void Update()
    {
        UpdateClock();

        UpdateLiveBlink();

        UpdateWorkerStats();
    }

    // ==================================================
    // CLOCK
    // ==================================================

    void UpdateClock()
    {
        DateTime now =
            DateTime.Now;

        // TIME
        timeText.text =
            now.ToString("hh:mm:ss tt");

        // DATE
        dateText.text =
            now.ToString("dd MMM yyyy");
    }

    // ==================================================
    // LIVE BLINK
    // ==================================================

    void UpdateLiveBlink()
    {
        blinkTimer += Time.deltaTime;

        if (blinkTimer >= 0.5f)
        {
            blinkTimer = 0;

            isVisible = !isVisible;

            liveDot.SetActive(isVisible);
        }
    }
    void UpdateWorkerStats()
    {
        int total =
            workers.Length;

        int active = 0;

        int idle = 0;

        int alerts = 0;

        foreach (WorkerStatus worker in workers)
        {
            // ACTIVE
            if (worker.currentState ==
                WorkerState.Working ||
                worker.currentState ==
                WorkerState.Walking)
            {
                active++;
            }

            // IDLE
            if (worker.currentState ==
                WorkerState.Idle)
            {
                idle++;
            }

            // ALERTS
            if (worker.currentState ==
                WorkerState.Restricted)
            {
                alerts++;
            }
        }

        totalWorkersText.text =
            total.ToString();

        activeWorkersText.text =
            active.ToString();

        idleWorkersText.text =
            idle.ToString();

        alertsText.text =
            alerts.ToString();
    }

}