using FluentAssertions;
using FluentValidation.TestHelper;
using GreenSpec.Application.Commands;
using GreenSpec.Application.Validators;

namespace GreenSpec.Tests.Unit.Application.Validators;

public class UpdateConfigCommandValidatorTests
{
    private readonly UpdateConfigCommandValidator _validator;

    public UpdateConfigCommandValidatorTests()
    {
        _validator = new UpdateConfigCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new UpdateConfigCommand(30m, 80m);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Validate_WithInvalidTempMax_ShouldHaveError(decimal tempMax)
    {
        // Arrange
        var command = new UpdateConfigCommand(tempMax, 80m);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.TempMax)
            .WithErrorMessage("Temperature threshold must be greater than 0");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithInvalidHumidityMaxTooLow_ShouldHaveError(decimal humidityMax)
    {
        // Arrange
        var command = new UpdateConfigCommand(30m, humidityMax);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.HumidityMax)
            .WithErrorMessage("Humidity threshold must be greater than 0");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(150)]
    public void Validate_WithInvalidHumidityMaxTooHigh_ShouldHaveError(decimal humidityMax)
    {
        // Arrange
        var command = new UpdateConfigCommand(30m, humidityMax);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.HumidityMax)
            .WithErrorMessage("Humidity threshold must be less than or equal to 100");
    }

    [Fact]
    public void Validate_WithBothInvalidValues_ShouldHaveMultipleErrors()
    {
        // Arrange
        var command = new UpdateConfigCommand(-1m, 101m);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.TempMax);
        result.ShouldHaveValidationErrorFor(c => c.HumidityMax);
    }

    [Theory]
    [InlineData(0.1, 0.1)]
    [InlineData(50, 50)]
    [InlineData(100, 100)]
    public void Validate_WithValidEdgeCases_ShouldNotHaveErrors(decimal tempMax, decimal humidityMax)
    {
        // Arrange
        var command = new UpdateConfigCommand(tempMax, humidityMax);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
