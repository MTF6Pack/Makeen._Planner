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

    public record ResetPasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Newpassword { get; set; }
    }
}
