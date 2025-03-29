using System;
using SQLite;

[Table("Users")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }

    //The email will be used as the username -> unique
    public string Email { get; set; }
    public string Role { get; set; }
    public string Token { get; set; }
    public DateTime LastLogin { get; set; }
}

public class PasswordHelper
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    public static bool ValidatePassword(string password, string correctHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, correctHash);
    }
}