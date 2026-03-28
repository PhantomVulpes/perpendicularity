using System.Text.RegularExpressions;

namespace Vulpes.Perpendicularity.Api.Configuration;

public class LowercaseParameterTransformer : IOutboundParameterTransformer
{
    public string? TransformOutbound(object? value)
    {
        return value?.ToString()?.ToLowerInvariant();
    }
}
