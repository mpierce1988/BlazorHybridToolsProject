using System.Collections.Specialized;

namespace ResultSetInterpreter.Models.ObjectDefinition;

public class ObjectDefinition
{
    public string? Name { get; set; }
    public List<Property> Properties { get; set; } = new();
    public List<OrderedDictionary> Objects { get; set; } = new();
}