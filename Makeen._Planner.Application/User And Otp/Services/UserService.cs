using Application.DataSeeder;
using Application.DataSeeder.OTP;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Repository.Interface;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Application.UserAndOtp.Services
{
    public class UserService(UserManager<User> userManager, SignInManager<User> signInManager, IUserRepository repository, JwtToken jwt, IOTPService emailOTPService) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly IOTPService _emailOTPService = emailOTPService;
        private readonly IUserRepository _repository = repository;
        private readonly JwtToken _jwt = jwt;

        public async Task<object?> GetUserById(Guid id)
        {
            var theuser = await _userManager.Users.Where(u => u.Id == id)
        .Select(u => new { u.Id, u.UserName, u.Email, u.PhoneNumber, u.AvatarUrl }).FirstOrDefaultAsync();
            return theuser ?? throw new NotFoundException(nameof(theuser));
        }
        public async Task<object?> GetUserByEmail(string email)
        {
            var theuser = await _userManager.Users.Where(u => u.Email == email).
            Select(u => new { u.Id, u.UserName, u.Email, u.PhoneNumber, u.AvatarUrl }).FirstOrDefaultAsync();
            return theuser ?? throw new NotFoundException(nameof(theuser));
        }
        public List<object>? GetAllUsers()
        {
            var Userslist = new List<object>();
            var users = _userManager.Users;
            if (users != null) foreach (var user in users) Userslist.Add(new { user.UserName, user.PhoneNumber, user.Email, user.Id });
            return Userslist;
        }
        public async Task<string> SignUP(AddUserCommand command)
        {
            var findresult = _userManager.FindByEmailAsync(command.Email);
            if (findresult != null) throw new BadRequestException("Email already exists");
            _emailOTPService.SendOTP(command.Email);
            var theuser = command.ToModel();
            var result = await _userManager.CreateAsync(theuser, command.Password);
            if (!result.Succeeded) throw new BadRequestException("Password");
            else return _jwt.Generate(theuser.Id.ToString(), command.Email);
        }
        public async Task<IdentityResult> DeleteUser(Guid id)
        {
            var theuser = await _userManager.FindByIdAsync(id.ToString());
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            await _userManager.DeleteAsync(theuser);
            return IdentityResult.Success ?? throw new BadRequestException("the theuser was not deleted");
        }
        public async Task UpdateUser(Guid id, UpdateUserCommand command)
        {
            var theuser = await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User");
            theuser.UpdateUser(command.UserName ?? theuser.UserName!, command.Email ?? theuser.Email!,
                command.PhoneNumber ?? theuser.PhoneNumber!, command.AvatarUrl ?? theuser.AvatarUrl);
            await _userManager.UpdateAsync(theuser);
        }
        public async Task<string> Signin([EmailAddress] string email, string password)
        {
            SignInResult signinResult = new();
            var theuser = _repository.StraitAccess.Set<User>().FirstOrDefault(x => x.Email == email);
            if (theuser != null) { signinResult = await _signInManager.PasswordSignInAsync(theuser.UserName!, password, false, false); }
            if (!signinResult.Succeeded) { throw new BadRequestException(signinResult.ToString()); }
            else if (theuser != null && theuser.Email != null) { return _jwt.Generate(theuser.Id.ToString(), email); }
            throw new BadRequestException();
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