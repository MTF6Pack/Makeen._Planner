using Makeen.Planner.Domain.Domains;

namespace Makeen._Planner.Duty_Service
{
    public interface IDutyService
    {
        void AddDuty(AddDutyCommand command);
        Duty? GetObjectByName(string name);
        Task<List<Duty>?> GetAllDuties();
        void RemoveDuty(Guid id);
        void UpdateDuty(Guid id, UpdateDutyCommand command);

    }
}
