using System.ComponentModel.DataAnnotations;

namespace ExpenseTracker.Api.Dtos
{
    // 1. 讀取列表
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }

    //2. 新增交易分類
        public class CreateCategoryDto
    {
        [Required(ErrorMessage = "分類名稱必填")]
        [StringLength(50, ErrorMessage = "分類名稱不能超過 50 字")]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Expense";
    }

}
