using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class HttpTrafficDataProvider : MonoBehaviour, ITrafficDataProvider
{
    [SerializeField] private string baseUrl = "http://localhost:3000/v1/traffic/status";

    public async Task<TrafficResponseDto> GetTrafficStatusAsync()
    {
        var request = UnityWebRequest.Get(baseUrl);
        var operation = request.SendWebRequest();

        while (!operation.isDone)
            await Task.Yield();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Erro na API: {request.error}");
            return null;
        }

        var json = request.downloadHandler.text;
        var dto = JsonUtility.FromJson<TrafficResponseDto>(json);
        request.Dispose();
        return dto;
    }
}
