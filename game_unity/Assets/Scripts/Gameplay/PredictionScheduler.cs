using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionScheduler : MonoBehaviour
{
    [SerializeField] private HUDController hud;

    private readonly List<Coroutine> jobs = new();

    public void CancelAll()
    {
        foreach (var job in jobs)
        {
            if (job != null) StopCoroutine(job);
        }
        jobs.Clear();
    }

    /// Cria uma coroutine por predicao, cada uma dispara apos seu estimated_time.
    /// A HUD sempre mostra a PROXIMA predicao que vai ocorrer, por isso passamos nextStatus adiante.
    public void Schedule(List<PredictedStatusDto> predicted, Action<StatusDto> onApply)
    {
        CancelAll();
        if (predicted == null || predicted.Count == 0) return;

        predicted.Sort((a, b) => a.estimated_time.CompareTo(b.estimated_time));

        hud.SetNextPrediction(predicted[0].estimated_time / 1000f, predicted[0].predictions);

        for (int i = 0; i < predicted.Count; i++)
        {
            float delay = Mathf.Max(0f, predicted[i].estimated_time / 1000f);
            StatusDto nextStatus = (i + 1 < predicted.Count) ? predicted[i + 1].predictions : null;
            float nextTime = (i + 1 < predicted.Count) ? predicted[i + 1].estimated_time / 1000f : 0f;
            bool isLast = (i == predicted.Count - 1);

            jobs.Add(StartCoroutine(ApplyAfter(delay, predicted[i].predictions, onApply, nextStatus, nextTime, isLast)));
        }
    }

    private IEnumerator ApplyAfter(float seconds, StatusDto status, Action<StatusDto> onApply,
        StatusDto nextStatus, float nextTime, bool isLast)
    {
        yield return new WaitForSeconds(seconds);
        onApply?.Invoke(status);

        if (isLast)
            hud.ClearNextPrediction();
        else
            hud.SetNextPrediction(nextTime, nextStatus);
    }
}
