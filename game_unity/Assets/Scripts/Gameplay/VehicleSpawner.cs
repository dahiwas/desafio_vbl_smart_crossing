using System.Collections;
using UnityEngine;

public class VehicleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject vehiclePrefab;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Vector2[] directions;
    [SerializeField] private float despawnX = 14f;



    private float spawnInterval = 1f;  // segundos entre cada spawn
    private float vehicleSpeed = 8f;
    private Coroutine loop;

    // Setters externos para o json que vai receber. Clamp evita valores quebrados.
    public void SetSpawnInterval(float value) => spawnInterval = Mathf.Max(0.05f, value);
    public void SetVehicleSpeed(float value)   => vehicleSpeed  = Mathf.Max(0.1f,  value);

    private void OnEnable()
    {
        loop = StartCoroutine(SpawnLoop()); // inicia o loop ao ativar o objeto
    }

    private void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
    }

    // Spawna um veículo, espera spawnInterval, repete.
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnOne();
            yield return new WaitForSeconds(spawnInterval); // intervalo dinâmico
        }
    }
    // A função propriamente dita para spawnar
    private void SpawnOne()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int i = Random.Range(0, spawnPoints.Length);
        var go = Instantiate(vehiclePrefab, spawnPoints[i].position, Quaternion.identity);
        var vc = go.GetComponent<VehicleController>();
        vc.Init(directions[i], vehicleSpeed, despawnX);
    }
}
