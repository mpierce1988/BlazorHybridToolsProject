using ResultSetInterpreter.Services.Interfaces;

namespace ResultSetInterpreter.Services.Test;

public class EPPlusExcelToCSharpServiceUnitTests
{
    private readonly IExcelToCSharpService _excelToCSharpService = 
        new ExcelCSharpService(new EPPlusExcelCSharpWorkbookParser());

    [Fact]
    public async Task ConvertUserTrackingSample_SuccessfulResult()
    {
        // Arrange
        string excelPath = Path.Combine(Directory.GetCurrentDirectory(), "Samples", "User_Tracking.xlsx");
        
        await using Stream? fileStream = File.OpenRead(excelPath);
        
        // Act
        string result = await _excelToCSharpService.ConvertExcelToCSharpAsync(fileStream);
        
        // Assert
        string expectedResult = @"public class User
{
   public System.Int32? UserID { get; set; }
   public System.String? FirstName { get; set; }
   public System.String? LastName { get; set; }
   public System.String? Email { get; set; }
   public System.DateTime? DOB { get; set; }
}

public List<User> UserList = new List<User>() {
   new User {
       UserID = 1,
       FirstName = ""Tim"",
       LastName = ""Robinson"",
       Email = ""tim@robinson.com"",
       DOB = DateTime.Parse(""12/14/1972 00:00:00"", CultureInfo.InvariantCulture)
    },
   new User {
       UserID = 2,
       FirstName = ""John"",
       LastName = ""Titor"",
       Email = ""john@titor.com"",
       DOB = DateTime.Parse(""05/09/1983 00:00:00"", CultureInfo.InvariantCulture)
    },
   new User {
       UserID = 3,
       FirstName = ""Steve"",
       LastName = ""Rogers"",
       Email = ""steve.rogers@avengers.org"",
       DOB = DateTime.Parse(""07/04/1918 00:00:00"", CultureInfo.InvariantCulture)
    }
};
public class Tracking
{
   public System.Int32? TrackingID { get; set; }
   public System.Int32? OrderID { get; set; }
   public System.String? IsActive { get; set; }
   public System.DateTime? DeliveryDate { get; set; }
}

public List<Tracking> TrackingList = new List<Tracking>() {
   new Tracking {
       TrackingID = 1,
       OrderID = 35,
       IsActive = ""True"",
       DeliveryDate = DateTime.Parse(""12/04/2022 00:00:00"", CultureInfo.InvariantCulture)
    },
   new Tracking {
       TrackingID = 2,
       OrderID = 54,
       IsActive = ""False"",
       DeliveryDate = DateTime.Parse(""01/22/2023 00:00:00"", CultureInfo.InvariantCulture)
    },
   new Tracking {
       TrackingID = 3,
       OrderID = 76,
       IsActive = ""True"",
       DeliveryDate = DateTime.Parse(""04/28/2023 00:00:00"", CultureInfo.InvariantCulture)
    },
   new Tracking {
       TrackingID = 4,
       OrderID = 89,
       IsActive = ""True"",
       DeliveryDate = DateTime.Parse(""05/04/2023 00:00:00"", CultureInfo.InvariantCulture)
    },
   new Tracking {
       TrackingID = 5,
       OrderID = 93,
       IsActive = ""False"",
       DeliveryDate = DateTime.Parse(""01/01/2024 00:00:00"", CultureInfo.InvariantCulture)
    }
};
";
        Assert.Equal(expectedResult, result);
        
    }
}