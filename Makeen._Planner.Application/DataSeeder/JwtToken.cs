using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace Application.DataSeeder
{
    public class JwtToken(IConfiguration configuration)
    {
        private string? Secretkey { get; set; } = configuration["JWT:Key"];

        public string Generate(string userId, string username)
        {
            if (userId != null && username != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(Secretkey!);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    [
                        new Claim(JwtRegisteredClaimNames.Sub,"id"),
                    new Claim("id", userId),
                    new Claim("userName", username)
                    ]),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            else return "Invalid request";
        }
    }
}
