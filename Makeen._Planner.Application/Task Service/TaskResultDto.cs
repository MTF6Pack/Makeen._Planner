using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Task_Service
{
    public record TaskResultDto
    {
        public bool HasConflict { get; set; }
        public string Message { get; set; } = string.Empty;
        public Domain.Task.Task? Task { get; set; }
    }
}
