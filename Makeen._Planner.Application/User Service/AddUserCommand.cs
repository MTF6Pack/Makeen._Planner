using MediatR;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Makeen._Planner.Service
{
    public class AddUserCommand : IRequest
    {
        public required string UserName { get; set; }
        public required int Age { get; set; }
        public required string Phonenumber { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
