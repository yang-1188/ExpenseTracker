using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // 這是用來抓 Token 

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 啟用 JWT 授權機制。沒有 Token 就會被拒絕 (401) //Program.cs 設定 app.UseAuthentication()

    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // --- 取得目前登入使用者的 ID (小工具) ---
        private Guid GetUserId()
        {
            // User 是 Controller 內建的變數，代表「目前登入的使用者」
            // FindFirst(...) 會去 Token 的 Claims 裡面找資料
            var idString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(idString))
            {
                throw new UnauthorizedAccessException("Token 裡找不到 User ID，請重新登入");
            }

            return Guid.Parse(idString);
        }

        // 1. 查詢交易列表
        // GET: api/Transactions
        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            try
            {
                // A. 從 Token 抓出「是誰」在查
                Guid userId = GetUserId();

                // B. 請transactionServic只拿「這個人」的資料
                var transactions = await _transactionService.GetTransactionsAsync(userId);

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "查詢交易失敗", error = ex.Message });
            }
        }

        // 2. 新增交易
        // POST: api/Transactions
        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto request)
        {
            try
            {
                // A. 從 Token 抓出「是誰」在記帳
                Guid userId = GetUserId();

                // B. 交給transactionService處理
                var result = await _transactionService.CreateTransactionAsync(request, userId);

                // C. 回傳 201 Created 
                return StatusCode(201, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "新增交易失敗", error = ex.Message });
            }
        }

        // --- 3. 刪除交易 ---
        // DELETE: api/Transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(Guid id)
        {
            try
            {
                // A. 抓出是誰在刪除
                Guid userId = GetUserId();

                // B. 呼叫廚房
                await _transactionService.DeleteTransactionAsync(id, userId);

                // C. 刪除成功，通常回傳 204 No Content (代表成功但沒東西要回傳)
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                // 找不到這筆資料
                return NotFound(new { message = "找不到此交易" });
            }
            catch (UnauthorizedAccessException)
            {
                // 這筆資料不是你的 (試圖刪除別人的資料)
                return StatusCode(403, new { message = "你沒有權限刪除此交易" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "刪除失敗", error = ex.Message });
            }
        }

        // --- 4. 更新交易 ---
        // PUT: api/Transactions/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto request)
        {
            try
            {
                Guid userId = GetUserId();
                var result = await _transactionService.UpdateTransactionAsync(id, request, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "找不到此交易" });
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { message = "你沒有權限修改此交易" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新失敗", error = ex.Message });
            }
        }
    }
}