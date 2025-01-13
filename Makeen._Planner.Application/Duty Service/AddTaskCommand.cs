using Domains;
using MediatR;
using System.Runtime.CompilerServices;

namespace Makeen._Planner.Task_Service
{
    public class AddTaskCommand : IRequest
    {
        public required string Name { get; set; }
        public required DateTime DeadLine { get; set; }
        public required TaskCategory TaskCategory { get; set; }
        public required PriorityCategory PriorityCategory { get; set; }
        public required Guid UserId { get; set; }
    }
}
