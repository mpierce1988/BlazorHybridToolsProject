using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ResultSetInterpreter.Models.ExcelCSharp;

namespace ResultSetInterpreter.Services.Test.Utilities;

public static class TestUtility
{
    public static T DeserializeFromJSon<T>(string json)
    {
        var settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter> { new ExcelCSharpValueConverter() }
        };
        
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, settings)!;
    }
    
    public static string SerializeToJson<T>(T obj)
    {
        var settings = new JsonSerializerSettings()
        {
            Converters = new List<JsonConverter> { new ExcelCSharpValueConverter() }
        };
        
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj, settings);
    }

    public static string GetSamplePath(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Samples", fileName);
    }
}

public class ExcelCSharpValueConverter : JsonConverter<ExcelCSharpValue>
{
    public override void WriteJson(JsonWriter writer, ExcelCSharpValue? value, JsonSerializer serializer)
    {
        // Use default handling to write json
        writer.WriteStartObject();

        PropertyInfo[] properties = value.GetType().GetProperties();

        foreach (PropertyInfo prop in properties)
        {
            writer.WritePropertyName(prop.Name);
            serializer.Serialize(writer, prop.GetValue(value));
        }

        writer.WriteEndObject();
    }

    public override ExcelCSharpValue? ReadJson(JsonReader reader, Type objectType, ExcelCSharpValue? existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var token = JObject.Load(reader);
        var result = new ExcelCSharpValue();
        
        // Custom handling for the 'Value' property
        var valueToken = token["Value"];

        if (valueToken != null)
        {
            if (valueToken.Type == JTokenType.Integer)
            {
                // Force integer to a double
                result.Value = (double)valueToken.ToObject<long>();
            }
            else
            {
                // default handling for all other types
                result.Value = valueToken.ToObject<object>();
            }
        }

        return result;
    }
}