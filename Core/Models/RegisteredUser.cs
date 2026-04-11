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
            EditingToken = DateTime.UtcNow.ToLongDateString()
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
    Admin
}