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

        // 1. 取得分類列表
        public async Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId)
        {
            // 邏輯：撈出 (系統預設 UserId is NULL) OR (我自己的 UserId == userId)
            var categories = await _context.Categories
                .Where(c => c.UserId == null || c.UserId == userId)
                .ToListAsync();

            // 轉成 DTO (記得 Type )
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Type = c.Type  
            });
        }

        // 2. 新增分類
        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request, Guid userId)
        {
            var newCategory = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Type = request.Type, 
                UserId = userId      //綁定給當前使用者
            };

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();

            // 回傳剛剛建立的資料
            return new CategoryDto
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Type = newCategory.Type
            };
        }
    }
}