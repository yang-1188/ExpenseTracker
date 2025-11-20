using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  //api/Auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // 註冊
        // api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                await _authService.RegisterAsync(request);
                return Ok(new { message = "註冊成功" });
            }
            catch (BadHttpRequestException ex) 
            {
                var errorResponse = new
                {
                    message = "註冊失敗",
                    detail = ex.Message 
                };
                return BadRequest(errorResponse);
            }
            catch (Exception ex) 
            {
                var errorResponse = new
                {
                    message = "註冊時發生未預期的錯誤，請稍後再試",
                    outerError = ex.Message,
                    innerError = ex.InnerException?.Message
                };
                return StatusCode(500, errorResponse);
            }
        }

        // 登入
        // /api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                string token = await _authService.LoginAsync(request);
                return Ok(new { Token = token });
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Login failed: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"[INNER_ERROR] {ex.InnerException.Message}");
                return StatusCode(500, "登入時發生未預期的錯誤，請稍後再試");
            }
        }

        // --- 端點 3：Google 登入 ---
        //   HTTP 方法：POST
        //   路徑：/api/Auth/google-login
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto request)
        {
            try
            {
                // 呼叫廚房的 GoogleLoginAsync 方法
                // 廚房會負責驗證 Token、註冊/綁定帳號、並發給我們 JWT
                string token = await _authService.GoogleLoginAsync(request);

                // 成功：回傳 200 OK，並把 Token 包在 JSON 裡
                return Ok(new { Token = token });
            }
            catch (BadHttpRequestException ex)
            {
                // 預期錯誤 (例如 Google Token 無效)
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // 未預期錯誤
                Console.WriteLine($"[ERROR] Google Login failed: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"[INNER_ERROR] {ex.InnerException.Message}");

                return StatusCode(500, new { message = "Google 登入時發生未預期的錯誤，請稍後再試。" });
            }
        }

    }
}
