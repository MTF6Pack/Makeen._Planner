using Application.DataSeeder;
using Application.EmailConfirmation;
using Application.User_And_Otp.Commands;
using Azure.Core;
using Domain;
using Infrustucture;
using Makeen._Planner.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application.UserAndOtp.Services
{
    public class UserService(UserManager<User> userManager, JwtTokenService jwt, IEmailConfirmService emailSender) : IUserService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly JwtTokenService _jwt = jwt;
        private readonly IEmailConfirmService _emailSender = emailSender;

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
        public async Task<List<object>> GetAllUsers()
        {
            var Userslist = new List<object>();
            var users = await _userManager.Users.ToListAsync();
            if (users != null) foreach (var user in users) Userslist.Add(new { user.UserName, user.AvatarUrl, user.PhoneNumber, user.Email, user.Id });
            return Userslist;
        }
        public async Task<string> SignUP(AddUserCommand command)
        {
            var existingUser = await _userManager.FindByEmailAsync(command.Email);
            if (existingUser != null)
            {
                if (!existingUser.EmailConfirmed)
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    await _userManager.ResetPasswordAsync(existingUser, resetToken, command.ConfirmPassword);
                    return await SendEmailConfirmationAsync(existingUser);
                }
                else throw new BadRequestException("Email already exists");
            }
            var newuser = command.ToModel();
            var result = await _userManager.CreateAsync(newuser, command.Password);
            if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            return await SendEmailConfirmationAsync(newuser);
        }
        private async Task<string> SendEmailConfirmationAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"https://109.230.200.230:6969/api/v1/accounts/confirm-emails?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailSender.SendConfirmEmailAsync(user.Email!, "Confirm Your Email",
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
        public async Task UpdateUser(UpdateUserCommand command, Guid userid)
        {
            var theuser = await _userManager.FindByIdAsync(userid.ToString())
                ?? throw new NotFoundException("User");

            // Handle email change
            if (!string.IsNullOrWhiteSpace(command.Email))
            {
                string newEmail = command.Email.ToLower();

                if (!string.Equals(newEmail, theuser.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var existingUser = await _userManager.FindByEmailAsync(newEmail);
                    if (existingUser != null)
                        throw new BadRequestException("Email already in use");

                    var emailResult = await _userManager.SetEmailAsync(theuser, newEmail);
                    if (!emailResult.Succeeded)
                        throw new BadRequestException($"Failed to update email: {string.Join(", ", emailResult.Errors.Select(e => e.Description))}");
                }
            }

            // Handle username change
            if (!string.IsNullOrWhiteSpace(command.UserName))
            {
                string newUserName = command.UserName.Trim();

                if (!string.Equals(newUserName, theuser.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    if (await _userManager.FindByNameAsync(newUserName) != null)
                        throw new BadRequestException("The username has already been taken");

                    var usernameResult = await _userManager.SetUserNameAsync(theuser, newUserName);
                    if (!usernameResult.Succeeded)
                        throw new BadRequestException($"Failed to update username: {string.Join(", ", usernameResult.Errors.Select(e => e.Description))}");
                }
            }

            // Handle avatar update
            string avatarUrl = command.Avatarurl != null
                ? await IformfileToUrl.UploadFile(command.Avatarurl, theuser.Id)
                : theuser.AvatarUrl!;

            // Update other user details
            theuser.UpdateUser(command.UserName?.Trim() ?? theuser.UserName!,
                               theuser.Email!, // Ensured by previous validation
                               avatarUrl,
                               command.PhoneNumber ?? theuser.PhoneNumber!);

            var updateResult = await _userManager.UpdateAsync(theuser);
            if (!updateResult.Succeeded)
                throw new BadRequestException($"Failed to update user details: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
        }

        public async Task<string> Signin(SigninDto request)
        {
            User user = await _userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User");
            if (!await _userManager.IsEmailConfirmedAsync(user)) throw new UnauthorizedException("Please confirm your email before logging in");
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
            if (!isPasswordValid) throw new BadRequestException("Invalid credentials");
            return _jwt.GenerateToken(user);
        }
        public async Task<IdentityResult> ResetPassword(ForgetPasswordDto request)
        {
            var theuser = await _userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User");
            var isSamePassword = await _userManager.CheckPasswordAsync(theuser, request.ConfirmPassword);
            if (isSamePassword) throw new BadRequestException("New password cannot be the same as the previous password.");
            var token = await _userManager.GeneratePasswordResetTokenAsync(theuser);
            var result = await _userManager.ResetPasswordAsync(theuser, token, request.Password);
            if (!result.Succeeded) throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));
            return result;
        }
    }
}