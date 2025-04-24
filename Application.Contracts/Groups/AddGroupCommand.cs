using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Contracts.Groups
{
    public class AddGroupCommand
    {
        public required string Title { get; set; }
        public required string Color { get; set; }
        public bool Grouptype { get; set; }
        public IFormFile? AvatarUrl { get; set; }

    }
    public class GroupMapper(IFileStorageService avatarService)
    {
        private readonly IFileStorageService _avatarService = avatarService;

        public async Task<Group> ToModel(AddGroupCommand command, Guid ownerid)
        {
            string? avatar = null;
            if (command.AvatarUrl != null)
            {
                avatar = await _avatarService.UploadFileAsync(command.AvatarUrl);
            }

            return new Group(
                command.Title,
                avatar,
                command.Color,
                ownerid,
                command.Grouptype);
        }
    }
}
