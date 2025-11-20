using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;
//JWT 相關
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
//Google api
using Google.Apis.Auth;

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
            return GenerateJwtToken(user);
        }

        // --- Google 登入 ---
        public async Task<string> GoogleLoginAsync(GoogleLoginDto request)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // 1. 驗證 Google Token
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    // 讀取 secrets.json 裡的 ClientId，確保 Token 是發給我們這個 App 的
                    Audience = new List<string> { _config["Authentication:Google:ClientId"]! }
                };

                // 這行是關鍵！它會去 Google 伺服器檢查簽章
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);
            }
            catch (InvalidJwtException)
            {
                // 如果 Token 無效 (偽造或過期)，回傳錯誤
                throw new BadHttpRequestException("Invalid Google Token.");
            }

            // 2. 檢查此 Google ID 是否已存在資料庫
            var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleSubjectId == payload.Subject);

            if (user == null)
            {
                // 3. 如果沒找到 Google ID，檢查 Email 是否已存在 (傳統註冊過)
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == payload.Email);

                if (existingUser != null)
                {
                    // 情境 A：他以前用 Email 註冊過 -> 自動綁定 Google ID
                    existingUser.GoogleSubjectId = payload.Subject;

                    // (可選) 如果原本沒頭像，就用 Google 的
                    if (string.IsNullOrEmpty(existingUser.AvatarUrl))
                    {
                        existingUser.AvatarUrl = payload.Picture;
                    }

                    user = existingUser; // 把 user 指向這個舊帳號
                }
                else
                {
                    // 情境 B：完全的新用戶 -> 自動註冊
                    user = new User
                    {
                        Email = payload.Email,
                        DisplayName = payload.Name,
                        GoogleSubjectId = payload.Subject,
                        AvatarUrl = payload.Picture,
                        CreatedAt = DateTime.UtcNow
                        // PasswordHash 留空，因為是用 Google 登入
                    };
                    _context.Users.Add(user);
                }

                // 儲存變更 (綁定或新增)
                await _context.SaveChangesAsync();
            }

            // 4. 產生我們自己的 JWT Token
            return GenerateJwtToken(user);
        }

        // --- 私有方法：產生 JWT Token (抽取出來共用) ---
        private string GenerateJwtToken(User user)
        {
            var secretKey = _config["JwtSettings:SecretKey"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];
            var expirationInMinutes = _config.GetValue<int>("JwtSettings:ExpirationInMinutes");


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("DisplayName", user.DisplayName)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(expirationInMinutes);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

    }
}
