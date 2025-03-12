using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Group_Service
{
    public record AddUserByEmailDto
    {
        public Guid Groupid { get; set; }
        [EmailAddress]
        public required string Email { get; set; }

    }

    public record GroupWithTaskCountsDto
    {
        public bool IsUserAdmin { get; set; }
        public Guid GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Color { get; set; }
        public bool Grouptype { get; set; } = false;
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
    }
}
