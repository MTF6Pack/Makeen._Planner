using Domain;
using Microsoft.AspNetCore.Http;

namespace Makeen._Planner.Service
{
    public class AddUserCommand
    {
        public Guid? AvatarId { get; set; }
        public required int Age { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string UserName { get; set; }
        public required string Phonenumber { get; set; }
    }
    public static class UserMapper
    {
        public static User ToModel(this AddUserCommand command)
        {
            User user = new(command.UserName, command.Email, command.Age, command.Phonenumber, command.AvatarId);
            return user;
        }
    }
}
