using System;
using System.IO;
using CatalogueServer.Helpers;
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

    // crud methods

    public void Insert<T>(T obj)
    {
        _db.Insert(obj);
    }

    public void Update<T>(T obj)
    {
        _db.Update(obj);
    }

    public void Delete<T>(T obj)
    {
        _db.Delete(obj);
    }

    public void DeleteAll<T>()
    {
        _db.DeleteAll<T>();
    }

    public TableQuery<T> Table<T>() where T : new()
    {
        return _db.Table<T>();
    }

    public void AddUser(string email, string password, string role, string name, string surname, string token)
    {
        var user = new User
        {
            Email = email,
            Password = PasswordHelper.HashPassword(password),
            Role = role,
            Name = name,
            Surname = surname,
            Token = token,
            LastLogin = DateTime.Now
        };
        Insert(user);
    }



}
