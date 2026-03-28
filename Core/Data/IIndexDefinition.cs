namespace Vulpes.Perpendicularity.Core.Data;

public interface IIndexDefinition
{
    string Name { get; }
    Task CreateIndexAsync();
    Task<bool> ExistsAsync();
}
