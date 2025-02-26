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
}
