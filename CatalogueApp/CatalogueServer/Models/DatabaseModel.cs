using System;
using System.IO;
using SQLite;

/// <summary>
/// Manages the SQLite database connection and schema for the school management system.
/// </summary>
public class Database
{
    /// <summary>
    /// Gets the SQLite database connection.
    /// </summary>
    public SQLiteConnection Connection { get; }

    /// <summary>
    /// Initializes a new instance of the Database class.
    /// Creates database tables if they don't exist and enables foreign key support.
    /// </summary>
    /// <param name="connection">Optional existing SQLite connection. If null, creates a new connection.</param>
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