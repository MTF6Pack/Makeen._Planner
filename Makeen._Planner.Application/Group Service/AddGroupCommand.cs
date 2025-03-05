using Domain;
using Microsoft.AspNetCore.Http;

namespace Application.Group_Service
{
    public class AddGroupCommand
    {
        public string? AvatarUrl { get; set; }
        public required string Title { get; set; }
        public bool Grouptype { get; set; } = false;
        public required string Color { get; set; }
    }

    public static class GroupMapper
    {
        public static Group ToModel(this AddGroupCommand command, Guid ownerid)
        {
            Group group = new(
                command.Title,
                command.AvatarUrl,
                command.Color, ownerid,
                command.Grouptype);
            return group;
        }
    }
}
