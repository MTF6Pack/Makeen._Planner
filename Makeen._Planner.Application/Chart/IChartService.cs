
namespace Application
{
    public interface IChartService
    {
        Task<int> GetUserStatus(Guid userid);
    }
}