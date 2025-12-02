namespace ExpenseTracker.Api.Dtos
{
    // 1. 前端「新增」交易時傳來的資料 (輸入)
    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }

        // 前端傳分類 ID 過來 
        public Guid CategoryId { get; set; }

        // 前端傳帳戶 ID 過來 
        public Guid AccountId { get; set; }
    }

    // 2. 前端「查詢」列表時，我們回傳的資料 (輸出)
    public class TransactionResponseDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        //為了讓前端編輯時可以回填
        public Guid CategoryId { get; set; }
        public Guid AccountId { get; set; }
        // 把前端 ID 轉換成「名稱」，方便顯示
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = string.Empty; // Expense 或 Income
        public string AccountName { get; set; } = string.Empty;
    }

    // 3. 輸入：更新交易用 
    public class UpdateTransactionDto
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Notes { get; set; }
        public Guid CategoryId { get; set; }
        public Guid AccountId { get; set; }
    }
}
