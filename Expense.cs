namespace PersonalExpenseTracker;

public class Expense
{
    public int Id { get; set; } // ID on pakollinen poistamista varten
    public string Description { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}