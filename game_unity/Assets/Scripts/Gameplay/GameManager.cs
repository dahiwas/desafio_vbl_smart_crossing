using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private MonoBehaviour providerBehaviour;
    [SerializeField] private TrafficManager trafficManager;
    [SerializeField] private PredictionScheduler scheduler;
    [SerializeField] private TimerController timer;
    [SerializeField] private PlayerController player;
    [SerializeField] private VehicleSpawner vehicleSpawner;
    [SerializeField] private HUDController hud;

    private ITrafficDataProvider provider;
    private int level = 1;
    private bool gameOver;

    private async void Start()
    {
        provider = (ITrafficDataProvider)providerBehaviour;
        await StartLevelAsync();
    }

    private async System.Threading.Tasks.Task StartLevelAsync()
    {
        player.ResetToSpawn();
        player.EnableInput(true);
        vehicleSpawner.gameObject.SetActive(true);

        foreach (var v in FindObjectsByType<VehicleController>(FindObjectsSortMode.None))
            Destroy(v.gameObject);

        hud.SetLevel(level);
        hud.ShowResult("");

        var dto = await provider.GetTrafficStatusAsync();

        trafficManager.ApplyStatus(dto.current_status);
        hud.SetTraffic(dto.current_status.vehicleDensity, dto.current_status.averageSpeed, dto.current_status.weather);

        scheduler.Schedule(dto.predicted_status, trafficManager.ApplyStatus);

        // O tempo limite e o maior estimated_time da API (ms -> s).
        // Isso garante que todas as predicoes ocorram antes do game over.
        // Fallback de 20s caso a API nao retorne predicoes.
        float deadline = (dto.predicted_status != null && dto.predicted_status.Count > 0)
            ? dto.predicted_status.Max(p => p.estimated_time) / 1000f
            : 20f;
        timer.StartCountdown(deadline, OnGameOver);
    }

    private void Update()
    {
        hud.SetTime(timer.RemainingTime);

        if (gameOver && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            gameOver = false;
            level = 1;
            _ = StartLevelAsync();
        }
    }

    public async void OnPlayerWin()
    {
        scheduler.CancelAll();
        timer.Stop();
        hud.ShowResult("Vitória!");
        level++;
        await StartLevelAsync();
    }

    public void OnGameOver()
    {
        scheduler.CancelAll();
        timer.Stop();
        player.EnableInput(false);
        vehicleSpawner.gameObject.SetActive(false);

        foreach (var v in FindObjectsByType<VehicleController>(FindObjectsSortMode.None))
            Destroy(v.gameObject);

        gameOver = true;
        hud.ShowResult("Game Over!\nPressione Espaço para reiniciar");
    }
}
