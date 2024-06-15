namespace ResultSetInterpreter.Services.Utilities;

public static class EnumerableExtensions
{
    public static double Median(this IEnumerable<double> source)
    {
        List<double> sortedList = source.OrderBy(n => n).ToList();

        int count = sortedList.Count;

        if (count == 0)
        {
            throw new InvalidOperationException("Empty collection");
        }
        
        // if the count is odd, return the middle element
        if (count % 2 == 1)
        {
            // For example, if the count is 5, the third item, which is at index 2
            // int 5 (count) divided by int 2 is 2.5, but since we are using integers, the result is 2
            return sortedList[count / 2]; 
        }
        
        // if the count is even, return the average of the middle two elements
        // For example, if the count is 4, we average the second and third item, which are at index 1 and 2
        // The average of these two elements is (sortedList[1] + sortedList[2]) / 2
        int secondIndex = count / 2;
        int firstIndex = secondIndex - 1;
        return (sortedList[firstIndex] + sortedList[secondIndex]) / 2.0f;
        
    }
}