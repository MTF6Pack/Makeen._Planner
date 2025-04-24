using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Groups
{
    public class UpdateGroupCommand
    {
        public string? Title { get; set; }
        public IFormFile? AvatarUrl { get; set; }
        public string? Color { get; set; }
    }
}
