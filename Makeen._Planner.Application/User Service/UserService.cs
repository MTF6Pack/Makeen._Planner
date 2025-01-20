using Application.DataSeeder;
using Domain;
using Microsoft.AspNetCore.Identity;
using Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Makeen._Planner.Service
{
    public class UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository repository) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IUserRepository _repository = repository;

        public async Task<User?> GetUserById(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }
        public List<object>? GetAllUsers()
        {
            var Userslist = new List<object>();
            var users = _userManager.Users;
            if (users != null) foreach (var user in users) Userslist.Add(new { user.UserName, user.PhoneNumber, user.Age, user.Email, user.Id });
            return Userslist;
        }
        public async Task<IdentityResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null) await _userManager.DeleteAsync(user);
            return IdentityResult.Success;
        }
        public async void UpdateUser(Guid id, UpdateUserCommand command)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (command.UserName != null) user?.UpdateUser(command.UserName, command.Email, command.Age, command.PhoneNumber);
        }
        public async Task<string> GenerateToken([EmailAddress] string email, string password)
        {
            SignInResult signinResult = new();
            var theuser = _repository.StraitAccess().Set<User>().FirstOrDefault(x => x.Email == email);
            if (theuser != null) { signinResult = await _signInManager.PasswordSignInAsync(theuser.UserName!, password, false, false); }
            if (!signinResult.Succeeded) { throw new Exception(JsonSerializer.Serialize(signinResult)); }
            else if (theuser != null && theuser.Email != null) { return JwtToken.Generate(theuser.Id.ToString(), email); }
            throw new Exception("Invalid input");
        }
        public void SignUP(AddUserCommand command)
        {
            var result = _userManager.CreateAsync(command.ToModel(), command.Password).Result;
            if (!result.Succeeded) throw new Exception("Invalid Input");
        }

        //public Task ForgetPassword(string email)
        //{

        //}
    }
}