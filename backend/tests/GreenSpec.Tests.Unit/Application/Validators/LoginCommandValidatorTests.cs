using FluentAssertions;
using FluentValidation.TestHelper;
using GreenSpec.Application.Commands;
using GreenSpec.Application.Validators;

namespace GreenSpec.Tests.Unit.Application.Validators;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator;

    public LoginCommandValidatorTests()
    {
        _validator = new LoginCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginCommand("admin", "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidUsername_ShouldHaveError(string? username)
    {
        // Arrange
        var command = new LoginCommand(username!, "password123");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Username)
            .WithErrorMessage("Username is required");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithInvalidPassword_ShouldHaveError(string? password)
    {
        // Arrange
        var command = new LoginCommand("admin", password!);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Validate_WithBothInvalidValues_ShouldHaveMultipleErrors()
    {
        // Arrange
        var command = new LoginCommand("", "");

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Username);
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Fact]
    public void Validate_WithValidLongCredentials_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new LoginCommand(
            new string('a', 100),
            new string('b', 100));

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
