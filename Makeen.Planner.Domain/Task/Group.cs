using Domain.Report;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Task
{
    public class Group
    {
        public Group(string title, IFormFile avatarUrl, string color, Guid ownerId)
        {
            Id = Guid.NewGuid();
            Title = title;
            AvatarUrl = avatarUrl;
            Color = color;
            OwnerId = ownerId;
        }

        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        [NotMapped]
        public IFormFile? AvatarUrl { get; private set; }
        public string Color { get; private set; } = string.Empty;
        public List<User>? Users { get; private set; }
        public List<Task>? Tasks { get; private set; }
        public Guid OwnerId { get; private set; }
        public List<Chart>? Charts { get; private set; }

        public void UpdateGroup(string title, IFormFile avatarurl, string color)
        {
            Title = title;
            AvatarUrl = avatarurl;
            Color = color;
        }

        public Group()
        {

        }
    }
}
