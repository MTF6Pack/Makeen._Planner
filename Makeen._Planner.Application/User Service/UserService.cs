using Makeen._Planner.Application.DataSeeder;
using Makeen.Planner.Domain.Domains;
using Makeen.Planner.Persistence.Repository;
using Makeen.Planner.Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class UserService(IUserRepository userRepository, IUnitOfWork unitOfWork) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        public string AddUser(AddUserCommand command)
        {
            _userRepository.Add(command.ToModel());
            _unitOfWork.SaveChanges();
            return $"Done! here your Id: {command.ToModel().Id}";
        }
        public async Task<User?> GetUserById(Guid id)
        {
            return await _userRepository.GetObjectByIdAsync(id);
        }

        public async Task<List<User>?> GetAllUsers()
        {
            return await _userRepository.GetAll();
        }
        public void DeleteUser(Guid id, string password)
        {
            var user = _userRepository.GetObjectById(id);
            if (user != null && password == user.Password)
            {
                _userRepository.Delete(id);
                _unitOfWork.SaveChanges();
            }
        }

        public void UpdateUser(Guid id, UpdateUserCommand command)
        {
            var user = _userRepository.GetObjectById(id);
            if (command != null) user?.UpdateUser(command.Name);
            _unitOfWork.SaveChanges();
        }

        public string GenerateToken([EmailAddress] string email, string password)
        {
            var theuser = _userRepository.StraitAccess().FirstOrDefault(x => x.Email == email && x.Password == password);
            if (theuser != null && email != null)
            {
                var token = JwtToken.Generate(theuser.Id.ToString(), theuser.Email);
                return token;
            }
            return "Get lost!";

        }
    }
}