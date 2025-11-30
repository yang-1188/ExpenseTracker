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

        public async Task<List<LookupDto>> GetAccountsAsync(Guid userId)
        {
            // 邏輯：只抓取「屬於這個人 (UserId == userId)」的錢包
            return await _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => new LookupDto
                {
                    Id = a.Id,
                    Name = a.Name
                })
                .ToListAsync();
        }
    }
}
