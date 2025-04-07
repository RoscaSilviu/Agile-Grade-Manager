using System;
using System.IO;
using SQLite;

public class Database
{
    public SQLiteConnection Connection { get; }

    public Database(SQLiteConnection connection = null)
    {
        if (connection != null)
        {
            Connection = connection;
        }
        else
        {
            string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "school_database.db");
            Connection = new SQLiteConnection(dbPath);
        }

        // Enable foreign keys
        Connection.Execute("PRAGMA foreign_keys = ON;");

        // Create tables if they don't exist
        Connection.CreateTable<User>();
        Connection.CreateTable<Class>();
        Connection.CreateTable<Grade>();
        Connection.CreateTable<Assignment>();
        Connection.CreateTable<ClassStudent>();
    }
}