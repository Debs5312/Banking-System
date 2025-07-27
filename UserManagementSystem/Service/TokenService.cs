using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace UserManagementSystem.Service
{
    public class TokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            var tokenKey = _config["TokenKey"];
            if (string.IsNullOrEmpty(tokenKey) || Encoding.UTF8.GetBytes(tokenKey).Length < 32)
            {
                // HMAC-SHA256 requires a key of at least 256 bits (32 bytes).
                // Throwing an exception on startup is crucial for security.
                throw new InvalidOperationException("TokenKey is not configured or is not long enough for HMAC-SHA256 security.");
            }
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var credentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            // A long-lived refresh token can be used to get new access tokens without forcing the user to log in again.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}