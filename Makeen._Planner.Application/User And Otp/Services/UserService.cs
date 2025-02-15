using Application.DataSeeder;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Application.UserAndOtp.Services
{
    public class UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository repository, JwtToken jwt) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IUserRepository _repository = repository;
        private readonly JwtToken _jwt = jwt;

        public async Task<User?> GetUserById(Guid id)
        {
            var theuser = await _userManager.FindByIdAsync(id.ToString());
            return theuser ?? throw new NotFoundException(nameof(theuser));
        }
        public async Task<User?> GetUserByEmail(string email)
        {
            var theuser = await _userManager.FindByEmailAsync(email);
            return theuser ?? throw new NotFoundException(nameof(theuser));
        }
        public List<object>? GetAllUsers()
        {
            var Userslist = new List<object>();
            var users = _userManager.Users;
            if (users != null) foreach (var user in users) Userslist.Add(new { user.UserName, user.PhoneNumber, user.Age, user.Email, user.Id });
            return Userslist;
        }
        public void SignUP(AddUserCommand command)
        {
            var user = command.ToModel();
            var result = _userManager.CreateAsync(user, command.Password).Result;
            if (!result.Succeeded) throw new BadRequestExeption();
        }
        public async Task<IdentityResult> DeleteUser(Guid id)
        {
            var theuser = await _userManager.FindByIdAsync(id.ToString());
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            await _userManager.DeleteAsync(theuser);
            return IdentityResult.Success ?? throw new Exception("the user was not deleted");
        }
        public async Task UpdateUser(Guid id, UpdateUserCommand command)
        {
            if (command.Age < 8) throw new BadRequestExeption("Age must be more than 8");
            var theuser = await _userManager.FindByIdAsync(id.ToString());
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            if (command.UserName != null)
            {
                theuser.UpdateUser(command.UserName, command.Email, command.Age, command.PhoneNumber, command.AvatarId);
                await _userManager.UpdateAsync(theuser);
            }
            else throw new BadRequestExeption(nameof(command.UserName));
        }
        public async Task<string> Signin([EmailAddress] string email, string password)
        {
            SignInResult signinResult = new();
            var theuser = _repository.StraitAccess.Set<User>().FirstOrDefault(x => x.Email == email);
            if (theuser != null) { signinResult = await _signInManager.PasswordSignInAsync(theuser.UserName!, password, false, false); }
            if (!signinResult.Succeeded) { throw new Exception(JsonSerializer.Serialize(signinResult)); }
            else if (theuser != null && theuser.Email != null) { return _jwt.Generate(theuser.Id.ToString(), email); }
            throw new BadRequestExeption();
        }
        public async Task SigninByClaims(User user, Claim claims)
        {
            await _signInManager.SignInWithClaimsAsync(user, false, (IEnumerable<Claim>)claims);
        }
        public async void ChangePassword(Guid userId, string currentpassword, string newpassword)
        {
            var theuser = await _userManager.FindByIdAsync(userId.ToString());
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            await _userManager.ChangePasswordAsync(theuser!, currentpassword, newpassword);
            await _userManager.UpdateAsync(theuser!);
        }
    }
}