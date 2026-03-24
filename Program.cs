using Microsoft.Data.Sqlite;
using Dapper;
using PersonalExpenseTracker;
using System.Globalization;

string connectionString = "Data Source=expenses.db";

// Initialize Database with REAL type for decimals
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
    Console.WriteLine("0. Exit");
    Console.Write("Select an option: ");
    
    var choice = Console.ReadLine();

    if (choice == "1")
    {
        Console.Write("Description (e.g. Coffee): ");
        string desc = Console.ReadLine() ?? "No description";

        Console.Write("Amount (e.g. 3.50 or 5,95): ");
        string input = Console.ReadLine()?.Replace(',', '.') ?? "0";
        
        // Varmistaa että syötetty arvo voidaan muuntaa doubleksi, ja käyttää invariant culturea pisteen erottimena
        double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double amount);

        Console.Write("Category: ");
        string cat = Console.ReadLine() ?? "General";

        var newExpense = new Expense { 
            Description = desc, 
            Amount = amount, 
            Category = cat, 
            Date = DateTime.Now 
        };

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Execute("INSERT INTO Expenses (Description, Amount, Category, Date) VALUES (@Description, @Amount, @Category, @Date)", newExpense);
            Console.WriteLine($"[SUCCESS] Saved: {amount:F2}€");
        }
    }
    else if (choice == "2")
    {
        using (var connection = new SqliteConnection(connectionString))
        {
            var expenses = connection.Query<Expense>("SELECT * FROM Expenses").ToList();
            
            Console.WriteLine("\nALL EXPENSES:");
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"{"Date",-12} | {"Description",-20} | {"Amount",-10} | {"Category"}");
            Console.WriteLine("------------------------------------------------------------------");
            
            foreach (var e in expenses)
            {
                // Tulostetaan hinta aina kahdella desimaalilla
                Console.WriteLine($"{e.Date:yyyy-MM-dd} | {e.Description,-20} | {e.Amount,10:F2}€ | {e.Category}");
            }
            
            // Lasketaan summa kaikista (Business Logic)
            double total = expenses.Sum(e => e.Amount);
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"{"TOTAL:",-35} {total,10:F2}€");
        }
    }
    else if (choice == "0") break;
}