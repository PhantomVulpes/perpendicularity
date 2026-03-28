namespace Vulpes.Perpendicularity.Core.ValueObjects;

public record HashedString(string Value) : ValueObjectBase(Value)
{
    public static HashedString Empty { get; } = new(string.Empty);

    public static implicit operator string(HashedString hashedString) => hashedString.Value;
}