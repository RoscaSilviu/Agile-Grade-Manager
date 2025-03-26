using System;
using System.IO;
using SQLite;

public class Database
{
    private readonly SQLiteConnection _db;

    public Database()
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "school_database.db");
        _db = new SQLiteConnection(dbPath);

        // Enable foreign keys
        _db.Execute("PRAGMA foreign_keys = ON;");

        // Create tables
        _db.CreateTable<User>();
        _db.CreateTable<Class>();
        _db.CreateTable<Grade>();
        _db.CreateTable<Assignment>();
    }

    public SQLiteConnection GetConnection() => _db;
}
