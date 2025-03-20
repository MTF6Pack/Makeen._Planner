//////using Application.DataSeeder;
//////using Microsoft.AspNetCore.Mvc;
//////using Microsoft.Extensions.Logging;
//////using Microsoft.IdentityModel.Tokens;
//////using System.IdentityModel.Tokens.Jwt;
//////using System.Security.Claims;
//////using System.Text;

//////namespace Makeen._Planner.Controllers
//////{
//////    [ApiController]
//////    [Route("api/[controller]")]
//////    public class TokenValidationController : ControllerBase
//////    {
//////        private readonly ILogger<TokenValidationController> _logger;
//////        private readonly JwtSettings _jwtSettings;

//////        public TokenValidationController(ILogger<TokenValidationController> logger, JwtSettings jwtSettings)
//////        {
//////            _logger = logger;
//////            _jwtSettings = jwtSettings;
//////        }

//////        /// <summary>
//////        /// Validates the provided JWT token.
//////        /// </summary>
//////        /// <param name="request">Contains the token to validate.</param>
//////        /// <returns>A result indicating whether the token is valid, along with claims or error details.</returns>
//////        [HttpPost("validate")]
//////        public IActionResult ValidateToken([FromBody] TokenRequest request)
//////        {
//////            if (string.IsNullOrEmpty(request?.Token))
//////            {
//////                _logger.LogWarning("No token provided for validation.");
//////                return BadRequest(new { Valid = false, Error = "Token is required." });
//////            }

//////            var tokenHandler = new JwtSecurityTokenHandler();
//////            try
//////            {
//////                var validationParameters = new TokenValidationParameters
//////                {
//////                    ValidateIssuer = false,
//////                    ValidateAudience = false,
//////                    ValidateLifetime = true,
//////                    ValidateIssuerSigningKey = true,
//////                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
//////                    ClockSkew = TimeSpan.Zero
//////                };

//////                // This call throws if token is invalid.
//////                ClaimsPrincipal principal = tokenHandler.ValidateToken(request.Token, validationParameters, out SecurityToken validatedToken);

//////                // Log the valid claims.
//////                var claimsList = principal.Claims.Select(c => new { c.Type, c.Value });
//////                _logger.LogInformation("Token is valid. Claims: {@Claims}", claimsList);

//////                return Ok(new { Valid = true, Claims = claimsList });
//////            }
//////            catch (Exception ex)
//////            {
//////                _logger.LogError(ex, "Token validation failed.");
//////                return Unauthorized(new { Valid = false, Error = ex.Message });
//////            }
//////        }
//////    }

//////    /// <summary>
//////    /// Request object for token validation.
//////    /// </summary>
//////    public class TokenRequest
//////    {
//////        public string Token { get; set; }
//////    }
//////}