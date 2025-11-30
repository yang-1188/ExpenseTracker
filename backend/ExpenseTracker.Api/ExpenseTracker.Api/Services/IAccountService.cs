using ExpenseTracker.Api.Dtos;

namespace ExpenseTracker.Api.Services
{
    public interface IAccountService
    {
        // 取得該使用者的所有錢包
        Task<List<LookupDto>> GetAccountsAsync(Guid userId);
    }
}