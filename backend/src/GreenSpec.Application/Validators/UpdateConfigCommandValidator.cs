using FluentValidation;
using GreenSpec.Application.Commands;

namespace GreenSpec.Application.Validators;

public class UpdateConfigCommandValidator : AbstractValidator<UpdateConfigCommand>
{
    public UpdateConfigCommandValidator()
    {
        RuleFor(x => x.TempMax)
            .GreaterThan(0)
            .WithMessage("Temperature threshold must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Temperature threshold must be less than or equal to 100");

        RuleFor(x => x.HumidityMax)
            .GreaterThan(0)
            .WithMessage("Humidity threshold must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Humidity threshold must be less than or equal to 100");
    }
}
