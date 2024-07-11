namespace ResultSetInterpreter.Services.Test.Utilities;

public static class TestUtility
{
    public static T DeserializeFromJSon<T>(string json)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json)!;
    }
    
    public static string SerializeToJson<T>(T obj)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
    }

    public static string GetSamplePath(string fileName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), "Samples", fileName);
    }
}

