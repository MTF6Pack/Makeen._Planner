using Domains;
using Makeen._Planner.Service;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Application
{
    public class UserCommandHandler : IRequestHandler<AddUserCommand>, IRequestHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> _userManager;

        public UserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var result = _userManager.CreateAsync(request.ToModel(), request.Password).Result;
            if (result.Succeeded) ($"Done! here your Id: {request.ToModel().Id}");
            else throw new Exception("djangoooooooooooooooooooooooooooo");
        }

        public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Userid.ToString());
            if (request.UserName != null) user?.UpdateUser(request.UserName, request.Email, request.Age, request.PhoneNumber);
        }
    }
}
