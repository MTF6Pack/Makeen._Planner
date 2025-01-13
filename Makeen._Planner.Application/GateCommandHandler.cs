using Domains;
using Makeen._Planner.Service;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public class GateCommandHandler(UserManager<User> userManager) : IRequestHandler<AddUserCommand, string>, IRequestHandler<UpdateUserCommand>
    {
        private readonly UserManager<User> _userManager = userManager;

        public async Task<string> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var result = _userManager.CreateAsync(request.ToModel(), request.Password).Result;
            if (result.Succeeded) return ($"Done! here your Id: {request.ToModel().Id}");
            else throw new Exception("Invalid Input");
        }

        public async System.Threading.Tasks.Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.Userid.ToString());
            if (request.UserName != null) user?.UpdateUser(request.UserName, request.Email, request.Age, request.PhoneNumber);
        }
    }
}
