namespace TakipApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int? TransactionsId { get; set; }
        public int? Amount { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CategoryName { get; set; }
        
    }

    public class Transaction
    {
        public int TransactionsId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public string? CategoryName { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Type { get; set;}
    }

    public class Income
    {
        public int IncomeId { get; set; }
        public decimal TotalPrice { get; set;}
    }

    
}
