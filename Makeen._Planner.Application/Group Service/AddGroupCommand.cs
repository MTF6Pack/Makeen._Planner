using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Http;

namespace Application.Group_Service
{
    public class AddGroupCommand
    {
        public IFormFile? AvatarUrl { get; set; }
        public required string Title { get; set; }
        public bool Grouptype { get; set; } = false;
        public required string Color { get; set; }
    }

    public static class GroupMapper
    {
        public static async Task<Group> ToModel(this AddGroupCommand command, Guid ownerid)
        {
            string? avatar = null;
            if (command.AvatarUrl != null)
            {
                avatar = await IformfileToUrl.UploadFile(command.AvatarUrl, ownerid);
            }
            return new Group(
                command.Title,
                avatar,
                command.Color, ownerid,
                command.Grouptype);
        }
    }
}
