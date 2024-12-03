using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class AddUserCommand
    {
        public required string Name { get; set; }
        [EmailAddress]
        public required string Email { get; set; }
        [PasswordPropertyText]
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Confirm password dosent match")]
        public required string RepeatPassword { get; set; }
    }
}
