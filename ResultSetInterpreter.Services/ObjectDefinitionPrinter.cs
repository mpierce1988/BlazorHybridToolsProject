using System.Collections;
using System.Collections.Specialized;
using System.Text;
using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class ObjectDefinitionPrinter : IObjectDefinitionPrinter
{
    public async Task<string> ClassDefinitionToCSharpCodeAsync(ObjectDefinition definition)
    {
        StringBuilder sb = new();

        await Task.Run(() =>
        {
            // Create Class Definition
            sb.AppendLine($"public class {definition.Name} {{");
            // Create Property Declarations
            for (int i = 0; i < definition.Properties.Count; i++)
            {
                sb.AppendLine($"public {definition.Properties[i].Type!.Name} {definition.Properties[i].Name} " +
                              "{ get; set; }");
            }
//
            sb.Append('}');
        });
        
        return sb.ToString();
    }

    public async Task<string> DataToCSharpListCodeAsync(ObjectDefinition definition)
    {
        StringBuilder sb = new();

        await Task.Run(() =>
        {
            // Create list declaration
            sb.AppendLine($"List<{definition.Name}> {definition.Name}List = new() {{");
    
            // Loop through data and create a new object for each item
            for (int i = 0; i < definition.Objects.Count; i++)
            {
                sb.AppendLine($"new {definition.Name}() {{");

                var objectForClass = definition.Objects[i];

                for (int j = 0; j < objectForClass.Count; j++)
                {
                    Property key = objectForClass.Cast<DictionaryEntry>().ElementAt(j).Key as Property;
                    string? value = objectForClass.Cast<DictionaryEntry>().ElementAt(j).Value as string;

                    string displayValue = GetDisplayValueByProperty(key, value);

                    if (j != objectForClass.Count - 1)
                    {
                        sb.AppendLine($"{key.Name} = {displayValue},");
                    }
                    else
                    {
                        sb.AppendLine($"{key.Name} = {displayValue}");
                    }
                }
                

                // Close object initializer, adding a comma unless it's the last object
                if (i != definition.Objects.Count - 1)
                {
                    sb.AppendLine("},");
                }
                else
                {
                    sb.AppendLine("}");
                }
            }
            
            // Close list declaration
            sb.Append("};");
        });
        
        return sb.ToString();
        
    }

    private string GetDisplayValueByProperty(Property? key, string? value)
    {
        string result = value ?? "null";

        if (key?.Type == typeof(string))
        {
            result = $"\"{value}\"";
        }
        else if (key?.Type == typeof(DateTime))
        {
            result = $"DateTime.Parse(\"{value}\")";
        }
        else if (key?.Type == typeof(int))
        {
            result = $"{value}";
        }
        
        return result;
    }

    public async Task<string> ObjectDefinitionToCSharpCode(ObjectDefinition definition)
    {
        StringBuilder sb = new();

        sb.AppendLine(await ClassDefinitionToCSharpCodeAsync(definition));
        sb.AppendLine();
        sb.Append(await DataToCSharpListCodeAsync(definition));
        
        return sb.ToString();
    }
}