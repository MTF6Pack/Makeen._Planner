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
}
