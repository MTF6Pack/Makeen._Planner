using Domain;

namespace Application.Group_Service
{
    public static class GroupMapper
    {
        public static Group ToModel(this AddGroupCommand command)
        {
            Group group = new(command.Title, command.AvatarUrl, command.Color, command.OwnerId);
            return group;
        }
    }
}
