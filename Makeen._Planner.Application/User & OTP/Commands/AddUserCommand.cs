using Domain;
using Domain.Report;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class AddUserCommand
    {
        public required string UserName { get; set; }
        public required int Age { get; set; }
        public required string Phonenumber { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public IFormFile? Avatar { get; set; }
    }
    public static class UserMapper
    {
        public static User ToModel(this AddUserCommand command)
        {
            User user = new(command.UserName, command.Email, command.Age, command.Phonenumber);
            return user;
        }
    }
}
