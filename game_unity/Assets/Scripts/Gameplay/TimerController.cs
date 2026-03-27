using System;
using UnityEngine;

// Timer de contagem regressiva. Recebe duração e um callback,
// decrementa a cada frame e dispara o callback ao zerar.
public class TimerController : MonoBehaviour
{
    private float remainingTime;
    private bool running;
    private Action onTimeUp;

    public float RemainingTime => remainingTime;

    // Inicia o timer com X segundos e guarda o callback para quando chegar o próximo
    public void StartCountdown(float seconds, Action onTimeUpCallback)
    {
        remainingTime = seconds;
        onTimeUp = onTimeUpCallback;
        running = true;
    }

    public void Stop()
    {
        running = false;
    }

    private void Update()
    {
        if (!running) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            running = false;
            onTimeUp?.Invoke();
        }
    }
}
