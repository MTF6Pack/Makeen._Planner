using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Makeen._Planner.Service
{
    public class UpdateUserCommand
    {
        public string? Name { get; set; }
        public string Password { get; set; } = string.Empty;

    }
}
