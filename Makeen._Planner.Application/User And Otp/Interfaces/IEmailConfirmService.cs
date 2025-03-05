namespace Application.EmailConfirmation
{
    public interface IEmailConfirmService
    {
        Task SendConfirmEmailAsync(string email, string subject, string messageBody);
    }
}