using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
//JWT 相關
using Microsoft.IdentityModel.Tokens; 
using System.IdentityModel.Tokens.Jwt; 
using System.Security.Claims;       
using System.Text;                  

namespace ExpenseTracker.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly ExpenseTrackerDBContext _context; 
        private readonly IConfiguration _config;         

        public AuthService(ExpenseTrackerDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // 1. 檢查 Email 是否重複
        public async Task RegisterAsync(RegisterRequestDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                throw new BadHttpRequestException($"Email '{request.Email}' is already taken.");
            }

            // 2. 加密密碼 
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

          
            var newUser = new User 
            {
                Email = request.Email,
                DisplayName = request.DisplayName,
                PasswordHash = passwordHash, 
                CreatedAt = DateTime.UtcNow 
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }


        public async Task<string> LoginAsync(LoginRequestDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                throw new BadHttpRequestException("Invalid credentials.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                throw new BadHttpRequestException("Invalid credentials.");
            }

            // --- 產生 JWT Token  ---

            var secretKey = _config["JwtSettings:SecretKey"]; 
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var expirationInHours = _config.GetValue<int>("JwtSettings:ExpirationInHours"); 
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("DisplayName", user.DisplayName) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddHours(expirationInHours); // 使用 UTC 時間

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            string jwtTokenString = tokenHandler.WriteToken(token);

            return jwtTokenString;
        }
    }
}
