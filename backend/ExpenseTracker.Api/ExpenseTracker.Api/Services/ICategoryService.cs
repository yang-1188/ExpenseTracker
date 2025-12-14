using ExpenseTracker.Api.Dtos;

namespace ExpenseTracker.Api.Services
{
    public interface ICategoryService
    {
        // 1. 取得分類列表 (包含系統預設 + 使用者自訂)
        Task<IEnumerable<CategoryDto>> GetCategoriesAsync(Guid userId);

        // 2. 新增分類
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto request, Guid userId);
    }
}