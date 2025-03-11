using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.User_And_Otp.Commands
{
    public record SigninDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public record CheckOtpDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string UserInput { get; set; }
    }

    public record ForgetPasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }

    public record UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Fullname { get; set; }
    }

    public record InviteUserDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Fullname { get; set; }
        public required string InviteCode { get; set; }
    }
}
