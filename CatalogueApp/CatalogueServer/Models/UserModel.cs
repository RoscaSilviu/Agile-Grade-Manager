using System;
using SQLite;

/// <summary>
/// Represents a user in the school management system.
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the hashed password for the user.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the user's surname.
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    /// Gets or sets the user's email address.
    /// Used as the username for authentication.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Gets or sets the user's role (e.g., "teacher", "student").
    /// </summary>
    public string Role { get; set; }

    /// <summary>
    /// Gets or sets the authentication token for the user.
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Gets or sets the timestamp of the user's last login.
    /// </summary>
    public DateTime LastLogin { get; set; }
}

/// <summary>
/// Helper class for password hashing and verification.
/// </summary>
public class PasswordHelper
{
    /// <summary>
    /// Hashes a plain text password using BCrypt.
    /// </summary>
    /// <param name="password">The plain text password to hash.</param>
    /// <returns>The hashed password.</returns>
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Validates a password against its hash.
    /// </summary>
    /// <param name="password">The plain text password to verify.</param>
    /// <param name="correctHash">The hash to verify against.</param>
    /// <returns>True if the password matches the hash, false otherwise.</returns>
    public static bool ValidatePassword(string password, string correctHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, correctHash);
    }
}