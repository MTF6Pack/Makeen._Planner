using Domain;
using Infrustucture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.DataSeeder
{

    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
    public class JwtTokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        public string? Secretkey { get; set; } = configuration["JWT:Key"];
        public UserManager<User> UserManager { get; set; } = userManager;

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secretkey!);
            var userclaims = new ClaimsIdentity(
                [
                    new Claim("id", user.Id.ToString()),
                    new Claim("email",user.Email!)
                ]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = userclaims,

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}