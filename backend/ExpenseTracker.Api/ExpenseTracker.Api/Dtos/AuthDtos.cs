namespace ExpenseTracker.Api.Dtos
{
    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    // Google 登入時，前端傳給我們的資料包
    public class GoogleLoginDto
    {
        // 前端 vue3-google-login 傳來的 "credential" 字串，就是這個 ID Token
        public string IdToken { get; set; } = string.Empty;
    }
}
