using Microsoft.IdentityModel.Tokens;
using RedmineApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace RedmineApp.Services.Implement
{
    public class TokenService : ITokenService
       
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string ComputeSha256Hash(string rawData)
        {
            using(SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for(int i=0; i<bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string GenerateAccessToken(int userId, string username, string refreshTokenId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expireMinutes = int.Parse(jwtSettings["ExpireMinutes"]!);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("user_id", userId.ToString()),
                new Claim("refresh_token_id", refreshTokenId),
                new Claim(ClaimTypes.Name, username)
            };
            var token = new JwtSecurityToken(
               issuer: jwtSettings["Issuer"],
               audience: jwtSettings["Audience"],
               claims: claims,
               expires: DateTime.Now.AddMinutes(expireMinutes),
               signingCredentials: creds
           );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string RawToken, string TokenHash, DateTime ExpiresAt) GenerateRefreshToken()
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var expireDays = int.Parse(jwtSettings["RefreshToken:ExpireDays"]!);
            // 1. Sinh ra chuỗi ngẫu nhiên 64 byte (Không thể đoán được)
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            string rawToken = Convert.ToBase64String(randomNumber);
            //// 2. Băm chuỗi đó ra (Mã hóa 1 chiều) trước khi lưu vào Database
            //string tokenHash = BCrypt.Net.BCrypt.HashPassword(rawToken);
            string tokenHash = ComputeSha256Hash(rawToken);
            DateTime ExpiresAt = DateTime.UtcNow.AddDays(expireDays);
            return (rawToken, tokenHash, ExpiresAt);
        }

        public JwtSecurityToken ParseToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(token);
        }

        public bool ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidIssuer = _configuration["JwtSettings:Issuer"],

                ValidateAudience = true,
                ValidAudience = _configuration["JwtSettings:Audience"],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),

                RequireExpirationTime = true
            };

            try
            {
                IPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                if (validatedToken is JwtSecurityToken jwt &&
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
