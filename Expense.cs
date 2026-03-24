namespace PersonalExpenseTracker;

public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public double Amount { get; set; } // Double
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}