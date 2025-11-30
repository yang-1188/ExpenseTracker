using ExpenseTracker.Api.Dtos;

namespace ExpenseTracker.Api.Services
{
    public interface ICategoryService
    {
        // 取得所有分類 (包含系統預設 + 使用者自訂)
        Task<List<LookupDto>> GetCategoriesAsync(Guid? userId);
    }
}