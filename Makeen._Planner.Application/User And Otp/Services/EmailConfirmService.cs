using Application.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace Application.EmailConfirmation
{
    public class EmailConfirmService : EmailServiceBase, IEmailConfirmService
    {
        public async Task SendConfirmEmailAsync(string email, string subject, string messageBody)
        {
            await SendEmailAsync(email, subject, messageBody);
        }
    }
}