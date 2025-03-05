using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Group_Service
{
    public record AddGroupDto
    {
        public IFormFile? AvatarUrl { get; set; }
        public required string Title { get; set; }
        public bool Grouptype { get; set; } = false;
        public required string Color { get; set; }
    }
}
