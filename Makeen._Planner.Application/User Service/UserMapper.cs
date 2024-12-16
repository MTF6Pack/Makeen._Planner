using Domains;

namespace Makeen._Planner.Service

{
    public static class UserMapper
    {
        public static User ToModel(this AddUserCommand command)
        {
            User user = new(command.UserName, command.Email, command.Age, command.Phonenumber);
            return user;
        }
    }
}
