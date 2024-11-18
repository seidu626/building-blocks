#nullable disable
using FluentValidation.Results;

namespace BuildingBlocks;

public sealed class ValidationException : Exception
{
    public ValidationException(ValidationError validationError)
        : base("Validation error")
    {
        this.ValidationError = validationError;
    }

    public ValidationException(string message, IEnumerable<ValidationFailure> validationFailures)
        : base(message)
    {
        ValidationFailures = validationFailures.ToList();
    }

    public ValidationError ValidationError { get; }
    public List<ValidationFailure> ValidationFailures { get; }
}