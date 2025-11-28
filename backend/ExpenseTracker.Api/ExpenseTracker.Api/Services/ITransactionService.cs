using ExpenseTracker.Api.Dtos;

namespace ExpenseTracker.Api.Services
{
    public interface ITransactionService
    {
        // 1. 查詢該使用者的所有交易
        //    輸入：userId (從 Token 拿到的)
        //    輸出：交易列表 
        Task<List<TransactionResponseDto>> GetTransactionsAsync(Guid userId);

        // 2. 新增一筆交易
        //    輸入：交易資料 (DTO) + userId (是誰新增的)
        //    輸出：新增成功的完整資料 (包含自動產生的 ID)
        Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto request, Guid userId);

    }
}
