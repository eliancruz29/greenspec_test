using FluentAssertions;
using GreenSpec.Domain.Entities;

namespace GreenSpec.Tests.Unit.Domain.Entities;

public class UserTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateUser()
    {
        // Arrange
        var username = "testuser";
        var passwordHash = "hashedpassword123";

        // Act
        var user = User.Create(username, passwordHash);

        // Assert
        user.Should().NotBeNull();
        user.Username.Should().Be(username);
        user.PasswordHash.Should().Be(passwordHash);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidUsername_ShouldThrowArgumentException(string? username)
    {
        // Arrange
        var passwordHash = "hashedpassword123";

        // Act
        var act = () => User.Create(username!, passwordHash);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Username cannot be empty*")
            .And.ParamName.Should().Be("username");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidPasswordHash_ShouldThrowArgumentException(string? passwordHash)
    {
        // Arrange
        var username = "testuser";

        // Act
        var act = () => User.Create(username, passwordHash!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Password hash cannot be empty*")
            .And.ParamName.Should().Be("passwordHash");
    }

    [Fact]
    public void Create_WithValidLongUsername_ShouldCreateUser()
    {
        // Arrange
        var username = new string('a', 50);
        var passwordHash = "hashedpassword123";

        // Act
        var user = User.Create(username, passwordHash);

        // Assert
        user.Username.Should().Be(username);
    }

    [Fact]
    public void Create_WithValidLongPasswordHash_ShouldCreateUser()
    {
        // Arrange
        var username = "testuser";
        var passwordHash = new string('b', 200);

        // Act
        var user = User.Create(username, passwordHash);

        // Assert
        user.PasswordHash.Should().Be(passwordHash);
    }
}
