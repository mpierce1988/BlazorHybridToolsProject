using System.Collections;
using System.Text;
using ResultSetInterpreter.Models.ObjectDefinition;
using ResultSetIntrepreter.Services.Interfaces;

namespace ResultSetIntrepreter.Services;

public class ObjectDefinitionPrinter : IObjectDefinitionPrinter
{
    #region Public Methods
    
    /// <summary>Converts an ObjectDefinition to C# code</summary>
    /// <param name="definition">The ObjectDefinition to convert to C# code</param>
    /// <returns>The C# code representing the ObjectDefinition</returns>
    public async Task<string> ObjectDefinitionToCSharpCode(ObjectDefinition definition)
    {
        StringBuilder sb = new();

        sb.AppendLine(await ClassDefinitionToCodeAsync(definition));
        sb.AppendLine();
        sb.Append(await ObjectsToListCodeAsync(definition));
        
        return sb.ToString();
    }
    
    /// <summary>Converts an ObjectDefinition to a class definition in C# code</summary>
    /// <param name="definition">The ObjectDefinition to convert to a class definition</param>
    /// <returns>The class definition in C# code</returns>
    public async Task<string> ClassDefinitionToCodeAsync(ObjectDefinition definition)
    {
        StringBuilder sb = new();

        await Task.Run(() =>
        {
            // Create Class Definition
            sb.AppendLine($"public class {definition.Name} {{");
            // Create Property Declarations
            foreach (Property property in definition.Properties)
            {
                sb.AppendLine($"public {property.Type!.Name} {property.Name} " +
                              "{ get; set; }");
            }
            // Close Class Definition
            sb.Append('}');
        });
        
        return sb.ToString();
    }

    /// <summary>Converts a list of objects to a list of object initializers in C# code</summary>
    /// <param name="definition">
    /// The object definition containing the objects to convert to a list of object initializers
    /// </param>
    /// <returns>The list of object initializers in C# code</returns>
    /// <exception cref="InvalidCastException">
    /// Thrown when the key is not a Property or the value is not a string
    /// </exception>
    public async Task<string> ObjectsToListCodeAsync(ObjectDefinition definition)
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
                    Property key = objectForClass.Cast<DictionaryEntry>().ElementAt(j).Key as Property 
                                   ?? throw new InvalidCastException("Key is not a Property");
                    string value = objectForClass.Cast<DictionaryEntry>().ElementAt(j).Value as string 
                                   ?? throw new InvalidCastException("Value is not a string");
                    
                    bool isLastProperty = j == objectForClass.Count - 1;
                    string propertyInitializerString = GetPropertyInitializerString(value, key, isLastProperty);

                    sb.AppendLine(propertyInitializerString);
                }
                

                // Close object initializer, adding a comma unless it's the last object
                sb.AppendLine(i != definition.Objects.Count - 1 ? "}," : "}");
            }
            
            // Close list declaration
            sb.Append("};");
        });
        
        return sb.ToString();
    }
    
    #endregion
    
    #region Private Methods

    /// <summary>
    /// Gets the property initializer string for the object initializer code block 
    /// </summary>
    /// <param name="value">The value of the property</param>
    /// <param name="key">The property key</param>
    /// <param name="isLastProperty">Whether or not the property is the last property in the object initializer</param>
    /// <returns>The property initializer string</returns>
    private string GetPropertyInitializerString(string value, Property key, bool isLastProperty)
    {
        string displayValue = value;

        if (key.Type == typeof(string))
        {
            displayValue = $"\"{value}\"";
        }
        else if (key.Type == typeof(DateTime))
        {
            displayValue = $"DateTime.Parse(\"{value}\")";
        }
        else if (key.Type == typeof(int))
        {
            displayValue = $"{value}";
        }
        
        string propertyInitializer = $"{key.Name} = {displayValue}";
        
        if (!isLastProperty)
        {
            propertyInitializer += ",";
        }
        
        return propertyInitializer;
    }
    
    #endregion
}