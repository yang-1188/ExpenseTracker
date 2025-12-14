using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services
{
    public class AccountService : IAccountService
    {
        private readonly ExpenseTrackerDBContext _context;

        public AccountService(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        // 1. 取得帳戶列表
        public async Task<IEnumerable<AccountDto>> GetAccountsAsync(Guid userId)
        {
            // 邏輯：系統預設 OR 我自己的
            var accounts = await _context.Accounts
                .Where(a => a.UserId == null || a.UserId == userId)
                .ToListAsync();

            return accounts.Select(a => new AccountDto
            {
                Id = a.Id,
                Name = a.Name
            });
        }

        // 2. 新增帳戶
        public async Task<AccountDto> CreateAccountAsync(CreateAccountDto request, Guid userId)
        {
            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                UserId = userId, // 👈 綁定給當前使用者
                // InitialBalance 這裡暫時沒存，如果資料庫有欄位可以加：
                // Balance = request.InitialBalance 
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return new AccountDto
            {
                Id = newAccount.Id,
                Name = newAccount.Name
            };
        }
    }
}