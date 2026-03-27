using System.Threading.Tasks;

public interface ITrafficDataProvider
{
    Task<TrafficResponseDto> GetTrafficStatusAsync();
}
