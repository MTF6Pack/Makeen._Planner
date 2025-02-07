using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Group_Service
{
    public class UpdateGroupCommand
    {
        public required string Title { get; set; }
        public Guid? AvatarId { get; set; }
        public required string Color { get; set; }
        public Guid OwnerId { get; set; }
    }
}
