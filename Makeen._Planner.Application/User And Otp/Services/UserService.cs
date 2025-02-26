using Application.DataSeeder;
using Application.User_And_Otp.Commands;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.UserAndOtp.Services
{
    public class UserService(UserManager<User> userManager, JwtTokenService jwt, IEmailSender emailSender) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly JwtTokenService _jwt = jwt;
        private readonly IEmailSender _emailSender = emailSender;

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
            var findResult = await _userManager.FindByEmailAsync(command.Email);
            if (findResult != null)
            {
                if (findResult.EmailConfirmed == false)
                {
                    await _userManager.ResetPasswordAsync(findResult, await _userManager.GeneratePasswordResetTokenAsync(findResult), command.ConfirmPassword);
                    return await SendEmailConfirmation(findResult);
                }
                else throw new BadRequestException("Email already exists");
            }
            var theUser = command.ToModel();
            var result = await _userManager.CreateAsync(theUser, command.Password);
            if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            return await SendEmailConfirmation(theUser);
        }
        private async Task<string> SendEmailConfirmation(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://109.230.200.230:6969/api/v1/accounts/confirm-emails?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendEmailAsync(user.Email!, "Confirm Your Email",
            $"Please confirm your account by clicking <a href='{confirmationLink}'>here</a>.");
            return "User registered successfully. Please check your email to confirm your account.";
        }
        public async Task<IdentityResult> DeleteUser(Guid id)
        {
            var theuser = await _userManager.FindByIdAsync(id.ToString());
            if (theuser == null) throw new NotFoundException(nameof(theuser));
            await _userManager.DeleteAsync(theuser);
            return IdentityResult.Success ?? throw new BadRequestException("the theuser was not deleted");
        }
        public async Task UpdateUser(UpdateUserCommand command, string token)
        {
            var claims = _jwt.ValidateToken(token) ?? throw new UnauthorizedException();
            var id = JwtTokenService.GetUserIdFromPrincipal(claims);
            var theuser = await _userManager.FindByIdAsync(id.ToString()) ?? throw new NotFoundException("User");
            if (command.Email != theuser.Email) { if (await _userManager.FindByEmailAsync(command.Email) != null) throw new BadRequestException("Email already in use"); }
            theuser.UpdateUser(command.UserName ?? theuser.UserName!, command.Email ?? theuser.Email!,
                command.PhoneNumber ?? theuser.PhoneNumber!, command.AvatarUrl ?? theuser.AvatarUrl);
            await _userManager.UpdateAsync(theuser);
        }
        public async Task<string> Signin(SigninDto request)
        {
            User user = await _userManager.FindByEmailAsync(request.Email) ?? throw new BadRequestException("Invalid credentials");
            if (!await _userManager.IsEmailConfirmedAsync(user)) throw new BadRequestException("Please confirm your email before logging in");
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) throw new BadRequestException("Invalid credentials");
            return _jwt.GenerateToken(user.Id.ToString(), request.Email);
        }
        public async void ChangePassword(Guid userId, string currentpassword, string newpassword)
        {
            var theuser = await _userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("theuser not found");
            await _userManager.ChangePasswordAsync(theuser!, currentpassword, newpassword);
            await _userManager.UpdateAsync(theuser!);
        }

    }
}