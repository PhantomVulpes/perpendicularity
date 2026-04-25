using Vulpes.Electrum.Domain.Models;
using Vulpes.Electrum.Domain.Validation;
using Vulpes.Perpendicularity.Core.Logging;
using Vulpes.Perpendicularity.Core.ValueObjects;

namespace Vulpes.Perpendicularity.Core.Models;

public record RegisteredUser : AggregateRoot
{
    public static RegisteredUser Empty { get; } = new();
    public static RegisteredUser Default => Empty with
    {
        Key = Guid.NewGuid(),
        CreationDate = DateTimeOffset.UtcNow,
    };

    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Concats the first name and last initial into a display name for the user.
    /// </summary>
    public string DisplayName => $"{FirstName} {LastName[0]}";

    public HashedString PasswordHash { get; init; } = HashedString.Empty;

    public UserStatus Status { get; init; } = UserStatus.Unknown;

    public DateTimeOffset CreationDate { get; init; } = DateTimeOffset.MinValue;
    public DateTimeOffset LastLoginDate { get; init; } = DateTimeOffset.MinValue;

    public IEnumerable<DownloadMetric> DownloadMetrics { get; init; } = [];
    public IEnumerable<UploadMetric> UploadMetrics { get; init; } = [];

    public override string ToLogName() => $"{LastName} {FirstName} ({Key})";

    public AggregateRootValidationModel<RegisteredUser> Validate()
    {
        var validationBuilder = new ValidationBuilder()
            .InvalidIf(() => Key == Guid.Empty, () => new ElectrumValidationError(ErrorCodes.INVALID_EMPTY_VALUE, $"{nameof(Key)} cannot be empty."));

        return new(this, validationBuilder);
    }

    public SaveModel<RegisteredUser> PrepareForSave()
    {
        var validatedObject = (this with
        {
            // Set any values that should be changed like last modified date or something.
            EditingToken = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")
        }).Validate();

        return new(validatedObject, EditingToken);
    }

    public InsertModel<RegisteredUser> PrepareForInsert()
    {
        var validatedObject = (this with
        {
            EditingToken = DateTime.UtcNow.ToLongDateString()
        }).Validate();

        return new(validatedObject);
    }
}

public enum UserStatus
{
    Unknown,
    Inactive,
    Unapproved,
    Approved,
    Rejected,
    Admin
}

public record DownloadMetric
{
    public static DownloadMetric Empty { get; } = new();
    public static DownloadMetric Default => Empty with
    {
        DownloadDate = DateTime.UtcNow
    };

    public string Path { get; init; } = string.Empty;
    public long SizeBytes { get; init; } = int.MinValue;
    public DateTime DownloadDate { get; init; } = DateTime.MinValue;
}

public record UploadMetric
{
    public static UploadMetric Empty { get; } = new();
    public static UploadMetric Default => Empty with
    {
        UploadDate = DateTime.UtcNow
    };

    public string Path { get; init; } = string.Empty;
    public long SizeBytes { get; init; } = int.MinValue;
    public DateTime UploadDate { get; init; } = DateTime.MinValue;
}