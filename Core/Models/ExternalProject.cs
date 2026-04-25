using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Perpendicularity.Core.Logging;

namespace Vulpes.Perpendicularity.Core.Models;

public record ExternalProject : AggregateRoot
{
    public static ExternalProject Empty { get; } = new();
    public static ExternalProject Default => Empty with
    {
        Key = Guid.NewGuid()
    };

    public string ProjectName { get; init; } = string.Empty;
    public string ProjectUri { get; init; } = string.Empty;
    public string Tooltip { get; init; } = string.Empty;

    public AggregateRootValidationModel<ExternalProject> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."))
            .InvalidIf(() => string.IsNullOrWhiteSpace(ProjectName), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(ProjectName)} cannot be empty."))
            .InvalidIf(() => string.IsNullOrWhiteSpace(ProjectUri), () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(ProjectUri)} cannot be empty."));

        return new(this, validationBuilder);
    }

    public SaveModel<ExternalProject> PrepareForSave()
    {
        var validatedObject = (this with
        {
            // Set any values that should be changed like last modified date or something.
            EditingToken = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        }).Validate();

        return new(validatedObject, EditingToken);
    }

    public InsertModel<ExternalProject> PrepareForInsert()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToLongDateString()
        }).Validate();

        return new(validatedObject);
    }
}