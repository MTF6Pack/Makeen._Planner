using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.DataSeeder
{
    public class WebSocketAuthMiddleware(RequestDelegate next, TokenValidationParameters tokenValidationParameters)
    {
        private readonly RequestDelegate _next = next;
        private readonly TokenValidationParameters _tokenValidationParameters = tokenValidationParameters;

        public async Task Invoke(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var token = context.Request.Query["access_token"].FirstOrDefault();

                if (string.IsNullOrEmpty(token) || !ValidateToken(token, out ClaimsPrincipal? user))
                {
                    Console.WriteLine("❌ Invalid or missing token");
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Invalid or missing token.");
                    return;
                }
                context.User = user!;
            }

            await _next(context);
        }

        private bool ValidateToken(string token, out ClaimsPrincipal? user)
        {
            user = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken)
                {
                    user = principal;
                    return true;
                }
            }
            catch
            {
                return false;
            }

            return false;
        }
    }
}