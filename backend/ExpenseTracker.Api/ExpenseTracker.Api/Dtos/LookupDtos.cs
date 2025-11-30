namespace ExpenseTracker.Api.Dtos
{
    // 查表/參照，專門給下拉選單使用
    public class LookupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}