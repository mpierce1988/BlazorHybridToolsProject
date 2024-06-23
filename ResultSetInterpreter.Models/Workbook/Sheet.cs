namespace ResultSetInterpreter.Models.Workbook;

public class Sheet
{
    // Create a property to represent the sheet name
    public string? Name { get; set; }
    
    // Create a property to represent the sheet cells
    public object?[,]? Cells { get; set; }
}