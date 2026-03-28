namespace Vulpes.Perpendicularity.Core.ValueObjects;

public abstract record ValueObjectBase(string Value)
{
    public override string ToString() => Value;

}