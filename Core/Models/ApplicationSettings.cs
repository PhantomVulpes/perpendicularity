using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Perpendicularity.Core.Logging;

namespace Vulpes.Perpendicularity.Core.Models;

public record ApplicationSettings : AggregateRoot
{
    public static Guid GlobalApplicationSettingsKey { get; } = Guid.Parse("003e7a5d-cd4a-4ff5-9666-4768bf828fc6");

    public static ApplicationSettings Empty { get; } = new();
    public static ApplicationSettings Default { get; } = Empty with
    {
        Key = GlobalApplicationSettingsKey
    };

    /// <summary>
    /// The paths that approved users are able to access for downloads.
    /// </summary>
    public IEnumerable<DirectoryConfiguration> DownloadPaths { get; init; } = [];

    public AggregateRootValidationModel<ApplicationSettings> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."))
            .InvalidIf(() => Key != GlobalApplicationSettingsKey, () => new ElectrumValidationError(ErrorCodes.INVALID_VALUE, $"{nameof(ApplicationSettings)}.{nameof(Key)} must be {GlobalApplicationSettingsKey}."))
            ;

        return new(this, validationBuilder);
    }

    public SaveModel<ApplicationSettings> PrepareForSave()
    {
        var validatedObject = (this with
        {
            // Set any values that should be changed like last modified date or something.
            EditingToken = DateTime.UtcNow.ToLongDateString()
        }).Validate();

        return new(validatedObject, EditingToken);
    }

    public InsertModel<ApplicationSettings> PrepareForInsert()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToLongDateString()
        }).Validate();

        return new(validatedObject);
    }
}

public record DirectoryConfiguration(string Path, string Alias);