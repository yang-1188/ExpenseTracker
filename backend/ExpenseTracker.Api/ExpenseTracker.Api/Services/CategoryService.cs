using ExpenseTracker.Api.Dtos;
using ExpenseTracker.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ExpenseTrackerDBContext _context;

        public CategoryService(ExpenseTrackerDBContext context)
        {
            _context = context;
        }

        public async Task<List<LookupDto>> GetCategoriesAsync(Guid? userId)
        {
            // 邏輯：撈出 (UserId 是 NULL 的系統分類) OR (UserId 是我自己的分類)
            // 注意：userId 參數可能是 null (雖然我們 Controller 會傳，但防呆一下)

            var query = _context.Categories.AsQueryable();

            if (userId.HasValue)
            {
                query = query.Where(c => c.UserId == null || c.UserId == userId);
            }
            else
            {
                query = query.Where(c => c.UserId == null);
            }

            return await query
                .Select(c => new LookupDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }
    }
}