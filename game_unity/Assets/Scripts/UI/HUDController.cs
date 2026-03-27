/*
Arquivov para conseguir controlar a HUD
Basicamente aqui teremos todos elementos do HUD
*/

using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text trafficText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text nextPredictionText;
    [SerializeField] private TMP_Text resultText;

    public void SetLevel(int level)
    {
        levelText.text = $"Nível: {level}";
    }

    public void SetTraffic(float density, float avgSpeed, string weather)
    {
        trafficText.text = $"Densidade: {density:0.00}\nVelocidade: {avgSpeed:0.0} km/h\nClima: {weather}";
    }

    public void SetTime(float seconds)
    {
        timerText.text = $"Tempo: {seconds:0.0}s";
    }

    public void SetNextPrediction(float seconds, StatusDto status)
    {
        nextPredictionText.text = $"Próximo em {seconds:0.0}s:\nDensidade: {status.vehicleDensity:0.00}\nVelocidade: {status.averageSpeed:0.0} km/h\nClima: {status.weather}";
    }

    public void ClearNextPrediction()
    {
        nextPredictionText.text = "";
    }

    public void ShowResult(string message)
    {
        resultText.text = message;
    }
}
