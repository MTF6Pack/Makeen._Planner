using Application.DataSeeder;
using Domain;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Identity;
using Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Application.UserAndOtp.Services
{
    public class UserService(UserManager<Domain.User> userManager, SignInManager<Domain.User> signInManager, IUserRepository repository) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IUserRepository _repository = repository;

        public async Task<Domain.User?> GetUserById(Guid id) => await _userManager.FindByIdAsync(id.ToString());
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
        public async Task UpdateUser(Guid id, UpdateUserCommand command)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user != null && command.UserName != null)
            {
                user.UpdateUser(command.UserName, command.Email, command.Age, command.PhoneNumber);
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task<string> GenerateToken([EmailAddress] string email, string password)
        {
            SignInResult signinResult = new();
            var theuser = _repository.StraitAccess().Set<Domain.User>().FirstOrDefault(x => x.Email == email);
            if (theuser != null) { signinResult = await _signInManager.PasswordSignInAsync(theuser.UserName!, password, false, false); }
            if (!signinResult.Succeeded) { throw new Exception(JsonSerializer.Serialize(signinResult)); }
            else if (theuser != null && theuser.Email != null) { return JwtToken.Generate(theuser.Id.ToString(), email); }
            throw new Exception("Invalid input");
        }
        public void SignUP(AddUserCommand command)
        {
            var user = command.ToModel();
            var result = _userManager.CreateAsync(user, command.Password).Result;
            var AvatarData = new MemoryStream();
            File.WriteAllBytes("C:\\Users\\MTF\\source\\repos\\Main project Makeen_Planner\\Makeen._Planner\\" + user.Id + ".png", AvatarData.ToArray());
            command.Avatar!.CopyTo(AvatarData);
            if (!result.Succeeded) throw new Exception(result.Errors.First().Description);
        }
        public async void ChangePassword(Guid userId, string currentpassword, string newpassword)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            await _userManager.ChangePasswordAsync(user!, currentpassword, newpassword);
            await _userManager.UpdateAsync(user!);
        }
    }
}