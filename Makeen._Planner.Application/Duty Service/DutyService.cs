using Domains;
using Repository;
using Repository.Interface;

namespace Makeen._Planner.Duty_Service
{
    public class DutyService(IDutyRepository repository, IUnitOfWork unitOfWork) : IDutyService
    {
        private readonly IDutyRepository _repository = repository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public void AddDuty(AddDutyCommand command)
        {
            _repository.Add(command.ToModel());
            _unitOfWork.SaveChanges();
        }

        public async Task<List<Duty>?> GetAllDuties()
        {
            return await _repository.GetAll();
        }

        public Duty? GetObjectByName(string name)
        {
            Duty? duty = _repository.StraitAccess().FirstOrDefault(x => x.Name == name);
            return duty;
        }

        public void RemoveDuty(Guid id)
        {
            _repository.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void UpdateDuty(Guid id, UpdateDutyCommand command)
        {
            var theduty = _repository.GetObjectById(id);
            if (command != null)
            {
                theduty?.UpdateDuty(command.Name, command.DeadLine, command.DutyCategory, command.PriorityCategory);
            }
        }
    }
}
