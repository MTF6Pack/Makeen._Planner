using Domain;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class AddUserCommand
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }
    public static class UserMapper
    {
        public static User ToModel(this AddUserCommand command)
        {
            User user = new(command.Email);
            return user;
        }
    }
}
