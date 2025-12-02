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

        // --- 1. 查詢列表 ---
        public async Task<List<TransactionResponseDto>> GetTransactionsAsync(Guid userId)
        {
            return await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.TransactionDate)
                .Select(t => new TransactionResponseDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TransactionDate = t.TransactionDate,
                    Notes = t.Notes,
                    CategoryId = t.CategoryId,
                    AccountId = t.AccountId,  
                    // 自動對應名稱
                    CategoryName = t.Category.Name,
                    CategoryType = t.Category.Type,
                    AccountName = t.Account.Name
                })
                .ToListAsync();
        }

        // --- 2. 新增交易 (修正版：回傳完整資料) ---
        public async Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto request, Guid userId)
        {
            // A. 建立實體
            var newTransaction = new Transaction
            {
                UserId = userId,
                AccountId = request.AccountId,
                CategoryId = request.CategoryId,
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                Notes = request.Notes
            };

            // B. 存入資料庫
            _context.Transactions.Add(newTransaction);
            await _context.SaveChangesAsync();

            // C. (關鍵修正) 重新查詢一次，為了拿到 Category 和 Account 的名稱
            //    這樣回傳給前端的資料才是完整的，前端如果有需要直接顯示這筆新資料，就不會壞掉
            var completeTransaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .FirstAsync(t => t.Id == newTransaction.Id);

            // D. 轉換成 DTO 回傳
            return new TransactionResponseDto
            {
                Id = completeTransaction.Id,
                Amount = completeTransaction.Amount,
                TransactionDate = completeTransaction.TransactionDate,
                Notes = completeTransaction.Notes,
                CategoryId = completeTransaction.CategoryId,
                AccountId = completeTransaction.AccountId,
                // 讓這裡有值可以回傳
                CategoryName = completeTransaction.Category.Name,
                CategoryType = completeTransaction.Category.Type,
                AccountName = completeTransaction.Account.Name
            };
        }

        // --- 3. 刪除交易 ---
        public async Task DeleteTransactionAsync(Guid id, Guid userId)
        {
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Transaction not found");
            }

            // 安全檢查：只能刪除自己的資料
            if (transaction.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not allowed to delete this transaction.");
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
        }

        // --- 4. 更新交易 ---
        public async Task<TransactionResponseDto> UpdateTransactionAsync(Guid id, UpdateTransactionDto request, Guid userId)
        {
            // A. 先找出這筆交易
            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null) throw new KeyNotFoundException("Transaction not found");

            // B. 安全檢查：只能改自己的資料
            if (transaction.UserId != userId)
            {
                throw new UnauthorizedAccessException("You are not allowed to update this transaction.");
            }

            // C. 更新欄位
            transaction.Amount = request.Amount;
            transaction.TransactionDate = request.TransactionDate;
            transaction.Notes = request.Notes;
            transaction.CategoryId = request.CategoryId;
            transaction.AccountId = request.AccountId;

            // D. 存檔
            await _context.SaveChangesAsync();

            // E. (跟 Create 一樣) 重新查詢一次，拿到完整的關聯名稱 (CategoryName...)
            // 這樣前端更新後，列表顯示才會正確
            var completeTransaction = await _context.Transactions
                .Include(t => t.Category)
                .Include(t => t.Account)
                .FirstAsync(t => t.Id == id);

            return new TransactionResponseDto
            {
                Id = completeTransaction.Id,
                Amount = completeTransaction.Amount,
                TransactionDate = completeTransaction.TransactionDate,
                Notes = completeTransaction.Notes,
                CategoryId = completeTransaction.CategoryId,
                AccountId = completeTransaction.AccountId,
                CategoryName = completeTransaction.Category.Name,
                CategoryType = completeTransaction.Category.Type,
                AccountName = completeTransaction.Account.Name
            };
        }
    }
}