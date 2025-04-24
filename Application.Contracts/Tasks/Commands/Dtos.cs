using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Tasks.Commands
{
    public class InstanceDto
    {
        public DateTime OccurrenceDate { get; set; }
        public bool IsCompleted { get; set; }
    }
}
