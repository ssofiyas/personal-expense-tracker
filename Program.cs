// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using Microsoft.Data.Sqlite;
using Dapper;
using PersonalExpenseTracker;

// tietokanta polku
string connectionString = "Data Source=expenses.db";

Console.WriteLine("=== Personal Expense Tracker - Alustus ===");

// yhteys tietokantaan ja taulun luominen
using (var connection = new SqliteConnection(connectionString))
{
    // SQL 
    string createTableSql = @"
        CREATE TABLE IF NOT EXISTS Expenses (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Description TEXT NOT NULL,
            Amount DECIMAL NOT NULL,
            Category TEXT NOT NULL,
            Date TEXT NOT NULL
        );";

    connection.Execute(createTableSql);
    Console.WriteLine("[OK] Database and table initialized.");
}

Console.WriteLine("Day 1: Basic structure initialized!");