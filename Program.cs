using Microsoft.Data.Sqlite;
using Dapper;
using PersonalExpenseTracker;
using System.Globalization;

string connectionString = "Data Source=expenses.db";

// ALUSTUS
using (var connection = new SqliteConnection(connectionString))
{
    connection.Execute(@"
        CREATE TABLE IF NOT EXISTS Expenses (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Description TEXT NOT NULL,
            Amount REAL NOT NULL, 
            Category TEXT NOT NULL,
            Date TEXT NOT NULL
        );");
}

while (true)
{
    Console.WriteLine("\n--- PERSONAL EXPENSE TRACKER ---");
    Console.WriteLine("1. Add New Expense");
    Console.WriteLine("2. View All Expenses");
    Console.WriteLine("3. Delete an Expense");
    Console.WriteLine("4. Category Percentage Report");
    Console.WriteLine("0. Exit");
    Console.Write("\nSelect an option: ");
    
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Description: ");
        string desc = Console.ReadLine() ?? "No description";
        Console.Write("Amount (e.g. 12.50): ");
        string input = Console.ReadLine()?.Replace(',', '.') ?? "0";
        double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount);
        Console.Write("Category: ");
        string cat = Console.ReadLine() ?? "General";

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Execute("INSERT INTO Expenses (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)", 
                new { Description = desc, Amount = amount, Category = cat, Date = DateTime.Now });
            Console.WriteLine("[SUCCESS] Saved!");
        }
    }
    else if (choice == "2")
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            var expenses = connection.Query<Expense>("SELECT * FROM Expenses").ToList();
            Console.WriteLine("\nID   | DATE       | DESCRIPTION          | AMOUNT");
            Console.WriteLine("-------------------------------------------------");
            foreach (var e in expenses)
                Console.WriteLine($"{e.Id,-4} | {e.Date:yyyy-MM-dd} | {e.Description,-20} | {e.Amount,8:F2}€");
            
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"TOTAL: {expenses.Sum(x => x.Amount),32:F2}€");
        }
    }
    else if (choice == "3")
    {
        Console.Write("Enter ID to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                int affected = connection.Execute("DELETE FROM Expenses WHERE Id = @Id", new { Id = id });
                Console.WriteLine(affected > 0 ? "[SUCCESS] Deleted." : "[ERROR] ID not found.");
            }
        }
    }
    else if (choice == "4")
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            // Lasketaan koko summa
            double grandTotal = connection.ExecuteScalar<double>("SELECT SUM(Amount) FROM Expenses");

            if (grandTotal == 0)
            {
                Console.WriteLine("\n[INFO] No expenses to report yet.");
                continue;
            }

            var report = connection.Query<(string Cat, double Sum)>(
                "SELECT Category, SUM(Amount) FROM Expenses GROUP BY Category ORDER BY SUM(Amount) DESC");

            Console.WriteLine("\n--- CATEGORY PERCENTAGE REPORT ---");
            Console.WriteLine($"Total Spending: {grandTotal:F2}€\n");
            Console.WriteLine($"{"Category",-15} | {"Amount",-10} | {"Percentage"}");
            Console.WriteLine("---------------------------------------------------");

            foreach (var row in report)
            {
                double percentage = (row.Sum / grandTotal) * 100;
                Console.WriteLine($"{row.Cat,-15} | {row.Sum,8:F2}€ | {percentage,10:F1}%");
            }
        }
    }
    else if (choice == "0") break;
}