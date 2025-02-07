using Domain.Report;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Group_Service
{
    public class AddGroupCommand
    {
        public Guid OwnerId { get; set; }
        public Guid? AvatarId { get; set; }
        public required string Title { get; set; }
        public required string Color { get; set; }
    }

}
