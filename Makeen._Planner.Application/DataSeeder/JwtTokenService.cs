using Domain;
using Microsoft.Extensions.Configuration;
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

    public class JwtTokenService(IConfiguration configuration)
    {
        private readonly string Secretkey = configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT Key is missing");
        private readonly string Issuer = configuration["JWT:Issuer"] ?? throw new ArgumentNullException("JWT Issuer is missing");
        private readonly string Audience = configuration["JWT:Audience"] ?? throw new ArgumentNullException("JWT Audience is missing");

        public string GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Secretkey);

            var userclaims = new ClaimsIdentity([
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim("id", user.Id.ToString()),
               new Claim("email", user.Email!)
            ]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = userclaims,
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            Console.WriteLine($"Generated Token: {tokenString}"); // ✅ Debugging line
            return tokenString;
        }
    }
}