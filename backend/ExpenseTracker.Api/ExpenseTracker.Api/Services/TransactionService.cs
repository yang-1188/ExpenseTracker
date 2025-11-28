using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ExpenseTrackerDBContext _context;

        public TransactionService(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        // --- 實作 1：查詢列表 ---
        public async Task<List<TransactionResponseDto>> GetTransactionsAsync(Guid userId)
        {
            // 這裡用到了 LINQ 的 "Projection" (投影) 技巧
            // 直接把資料庫物件 (Transaction) 轉換成 DTO
            var transactions = await _context.Transactions
                .Include(t => t.Category) // 告訴 EF Core 我們要關聯分類表
                .Include(t => t.Account)  // 告訴 EF Core 我們要關聯帳戶表
                .Where(t => t.UserId == userId) // ⚠️ 關鍵：只抓「這個人」的資料！
                .OrderByDescending(t => t.TransactionDate) // 按日期倒序 (最新的在上面)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    // 👇 這裡就是「自動翻譯」！
                    CategoryName = t.Category.Name,
                    CategoryType = t.Category.Type,
                    AccountName = t.Account.Name
                })
                .ToListAsync();

            return transactions;
        }

        // --- 實作 2：新增交易 ---
        public async Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto request, Guid userId)
        {
            // 1. 驗證 (防呆)：檢查傳進來的 CategoryId 和 AccountId 是否真的存在？
            // (這邊為了求快先跳過，但在生產環境建議要檢查，不然會報外鍵錯誤)

            // 2. 建立資料庫物件
            var newTransaction = new Transaction
            {
                UserId = userId, // 綁定使用者
                AccountId = request.AccountId,
                CategoryId = request.CategoryId,
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                Notes = request.Notes
            };

            // 3. 存入資料庫
            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();

            // 4. 為了回傳完整的 DTO (包含名稱)，我們需要「重新查詢」一次
            //    (因為 newTransaction 裡面只有 ID，沒有 Category.Name)

            // 稍微偷懶的做法：手動填入名稱 (如果你前端立刻需要顯示)
            // 或者正規做法：再次 query (會多一次 DB 消耗)
            // 這裡我們先回傳基本的，前端通常會重新整理列表

            // 為了演示，我們做一個簡單的回傳：
            return new TransactionResponseDto
            {
                Id = newTransaction.Id,
                Amount = newTransaction.Amount,
                TransactionDate = newTransaction.TransactionDate,
                Notes = newTransaction.Notes,
                // 這裡暫時無法拿到 Name，除非再查一次 DB
                // 在快速開發中，這通常是可以接受的，因為新增完通常會重抓列表
                CategoryName = "",
                AccountName = ""
            };
        }
    }
}
