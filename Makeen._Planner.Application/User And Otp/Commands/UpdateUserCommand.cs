using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class UpdateUserCommand
    {
        public IFormFile? Avatarurl { get; set; }
        public string? UserName { get; set; }
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
    }
}
