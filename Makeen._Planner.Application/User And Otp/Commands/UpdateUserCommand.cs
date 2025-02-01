using Microsoft.AspNetCore.Http;

namespace Makeen._Planner.Service
{
    public class UpdateUserCommand
    {
        public string? UserName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }
    }
}
