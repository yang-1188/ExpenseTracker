using ExpenseTracker.Api.Dtos;

namespace ExpenseTracker.Api.Services
{
    public interface IAccountService
    {
        //1. 取得使用者帳戶列表
        Task<IEnumerable<AccountDto>> GetAccountsAsync(Guid userId);

        // 2. 新增帳戶
        Task<AccountDto> CreateAccountAsync(CreateAccountDto request, Guid userId);
    }
}