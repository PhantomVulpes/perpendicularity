using Vulpes.Electrum.Domain.Exceptions;
using Vulpes.Electrum.Domain.Security;

namespace Vulpes.Perpendicularity.Core.Models;

public abstract record AggregateRoot
{
    public Guid Key { get; init; } = Guid.Empty;

    public string EditingToken { get; init; } = DateTime.MinValue.ToString();

    public virtual string ToLogName() => $"Key: {ToString()}";

    protected virtual ElectrumValidationResult InternalValidate() => ElectrumValidationResult.Verify(() => Key != Guid.Empty, $"{nameof(Key)} cannot be {Key}.");

    public void ValidateOrThrow()
    {
        var isValid = InternalValidate();

        if (!isValid)
        {
            throw new ElectrumValidationException(isValid);
        }
    }
}
