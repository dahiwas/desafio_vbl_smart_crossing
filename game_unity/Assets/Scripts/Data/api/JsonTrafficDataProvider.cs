using System.Threading.Tasks;
using UnityEngine;

public class JsonTrafficDataProvider : MonoBehaviour, ITrafficDataProvider
{
    [SerializeField] private TextAsset mockJson;

    public Task<TrafficResponseDto> GetTrafficStatusAsync()
    {
        var dto = JsonUtility.FromJson<TrafficResponseDto>(mockJson.text);
        return Task.FromResult(dto);
    }
}
