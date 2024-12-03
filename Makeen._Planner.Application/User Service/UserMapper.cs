using Makeen.Planner.Domain.Domains;

namespace Makeen._Planner.Service

{
    public static class UserMapper
    {
        public static User ToModel(this AddUserCommand command)
        {
            User user = new(command.Name, command.Email, command.Password, command.RepeatPassword);
            return user;
        }
    }
}
