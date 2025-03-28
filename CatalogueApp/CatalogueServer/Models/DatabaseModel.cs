using System;
using System.IO;
using SQLite;
using CatalogueServer.Repositories;

public class Database
{
    private readonly SQLiteConnection _db;

    public UserRepository Users { get; }
    public ClassRepository Classes { get; }
    public AssignmentRepository Assignments { get; }
    public GradeRepository Grades { get; }

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

        // Initialize repositories (use the correct types)
        Users = new UserRepository(_db);
        Classes = new ClassRepository(_db);
        Assignments = new AssignmentRepository(_db);
        Grades = new GradeRepository(_db);
    }

    public SQLiteConnection GetConnection() => _db;
}
