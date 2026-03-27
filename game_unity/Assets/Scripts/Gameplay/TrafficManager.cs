using UnityEngine;

// Recebe o StatusDto (vindo do JSON da API) e distribui os valores
public class TrafficManager : MonoBehaviour
{
    [SerializeField] private VehicleSpawner vehicleSpawner;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private HUDController hud;
    [SerializeField] private float referenceVehicleSpeed = 14f; // base para calcular a velocidade real

    // Chamado sempre que um novo status chega da API.
    public void ApplyStatus(StatusDto status)
    {
        // Converte os dados brutos do DTO em valores de gameplay via TrafficMath
        float spawn = TrafficMath.SpawnInterval(status.vehicleDensity);   // densidade -> intervalo de spawn
        float speed = TrafficMath.VehicleSpeed(status.averageSpeed, referenceVehicleSpeed); // velocidade média -> speed dos veículos
        float mult  = TrafficMath.WeatherMultiplier(status.weather);      // clima -> multiplicador de movimento do player

        // Aplica no veículo
        vehicleSpawner.SetSpawnInterval(spawn);
        vehicleSpawner.SetVehicleSpeed(speed);
        playerController.SetWeatherMultiplier(mult);
        hud.SetTraffic(status.vehicleDensity, status.averageSpeed, status.weather); // atualiza UI
    }
}
