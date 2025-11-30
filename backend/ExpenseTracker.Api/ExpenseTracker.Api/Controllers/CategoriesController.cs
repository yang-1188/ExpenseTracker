using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 需要登入，因為要抓 userId
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            // 從 Token 抓 UserID (複製之前 TransactionsController 的邏輯)
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idString)) return Unauthorized();
            var userId = Guid.Parse(idString);

            var list = await _service.GetCategoriesAsync(userId);
            return Ok(list);
        }
    }
}