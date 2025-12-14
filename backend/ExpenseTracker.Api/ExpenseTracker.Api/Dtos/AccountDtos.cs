using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Dtos
{
    // 1. 讀取列表
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    //2. 新增帳戶分類
    public class CreateAccountDto
    {
        [Required(ErrorMessage = "帳戶名稱必填")]
        [StringLength(50, ErrorMessage = "帳戶名稱不能超過 50 字")]
        public string Name { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; } = 0;
    }

}
