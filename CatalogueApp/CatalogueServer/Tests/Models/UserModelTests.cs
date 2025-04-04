using Xunit;

public class UserModelTests
{
    [Fact]
    public void HashPassword_ShouldCreateDifferentHashesForSamePassword()
    {
        // Arrange
        string password = "TestPassword123";

        // Act
        string hash1 = PasswordHelper.HashPassword(password);
        string hash2 = PasswordHelper.HashPassword(password);

        // Assert
        Assert.NotEqual(hash1, hash2); // BCrypt should generate different salts
        Assert.True(PasswordHelper.ValidatePassword(password, hash1));
        Assert.True(PasswordHelper.ValidatePassword(password, hash2));
    }

    [Fact]
    public void ValidatePassword_ShouldReturnTrue_ForCorrectPassword()
    {
        // Arrange
        string password = "TestPassword123";
        string hash = PasswordHelper.HashPassword(password);

        // Act
        bool isValid = PasswordHelper.ValidatePassword(password, hash);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public void ValidatePassword_ShouldReturnFalse_ForIncorrectPassword()
    {
        // Arrange
        string password = "TestPassword123";
        string wrongPassword = "WrongPassword123";
        string hash = PasswordHelper.HashPassword(password);

        // Act
        bool isValid = PasswordHelper.ValidatePassword(wrongPassword, hash);

        // Assert
        Assert.False(isValid);
    }
}