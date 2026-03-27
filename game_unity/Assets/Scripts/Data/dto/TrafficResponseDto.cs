using System;
using System.Collections.Generic;

[Serializable]
public class TrafficResponseDto
{
    public StatusDto current_status;
    public List<PredictedStatusDto> predicted_status;
}

[Serializable]
public class PredictedStatusDto
{
    public int estimated_time; // milissegundos
    public StatusDto predictions;
}

[Serializable]
public class StatusDto
{
    public float vehicleDensity;  // 0.1 a 1.0
    public float averageSpeed;    // 0 a 100 km/h
    public string weather;        // sunny, clouded, foggy, light rain, heavy rain
}
