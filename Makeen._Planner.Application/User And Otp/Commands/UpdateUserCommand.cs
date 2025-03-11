using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class UpdateUserCommand
    {
        public string? Fullname { get; set; }
        public IFormFile? Avatarurl { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
