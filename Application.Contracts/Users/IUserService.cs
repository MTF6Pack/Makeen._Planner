using Application.Contracts.Users.Commands;
using Microsoft.AspNetCore.Identity;

namespace Application.Contracts.Users
{
    public interface IUserService
    {
        Task<string> SignUP(AddUserCommand command);
        Task<object?> GetUserById(Guid id);
        Task<object?> GetUserByEmail(string email);
        Task<string> Signin(SigninDto request);
        Task<List<object>> GetAllUsers();
        Task UpdateUser(UpdateUserCommand command, Guid userid);
        Task<IdentityResult> DeleteUser(Guid id);
        Task<IdentityResult> ResetPassword(ForgetPasswordDto request);
        Task<string> InviteFriend(InviteUserDto request, string userid);
        Task AddContact(string theuserid, AddContactDto request);
        Task<List<GetContactDto>> GetContacts(string theuserid);
        Task DeleteContact(string theuserid, Guid targetuserid);
    }
}
