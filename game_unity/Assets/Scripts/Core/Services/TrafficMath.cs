using UnityEngine;

public static class TrafficMath
{
    // Densidade 1.0 = 1 veiculo/s | Densidade 0.1 = 1 veiculo a cada 10s
    public static float SpawnInterval(float density)
    {
        var d = Mathf.Clamp(density, 0.1f, 1f);
        return 1f / d;
    }

    public static float VehicleSpeed(float averageSpeed, float referenceSpeed)
    {
        var s = Mathf.Clamp(averageSpeed, 0f, 100f);
        return (s / 100f) * referenceSpeed;
    }

    // Penalidade de clima aplicada na velocidade do jogador (nao dos veiculos)
    public static float WeatherMultiplier(string weather)
    {
        return weather switch
        {
            "sunny"      => 1.0f,
            "clouded"    => 0.8f,
            "foggy"      => 0.8f,
            "light rain" => 0.6f,
            "heavy rain" => 0.4f,
            _            => 1.0f //evitar crash ou deveria manter
        };
    }
}
