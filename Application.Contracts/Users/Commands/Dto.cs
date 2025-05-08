using System.ComponentModel.DataAnnotations;

namespace Application.Contracts.Users.Commands
{
    public record SigninDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public record CheckOtpDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string UserInput { get; set; }
    }

    public record ForgetPasswordDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
    }

    public record UserDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Fullname { get; set; }
    }

    public record InviteUserDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public required string Fullname { get; set; }
        public required string InviteCode { get; set; }
    }

    public record GetContactDto
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Fullname { get; set; }
    }

    public record AddContactDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? TargetId { get; set; }
    }
}
