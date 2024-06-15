using System.Diagnostics;
using ResultSetInterpreter.Models.Benchmark;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Utilities;

namespace ResultSetIntrepreter.Services;

public class BenchmarkService : IBenchmarkService
{
    public async Task<BenchmarkResult> RunBenchmarkAsync(BenchmarkRequest benchmarkRequest)
    {
        // Validate a method was provided
        if (benchmarkRequest.Method == null)
        {
            throw new ArgumentException("Run Benchmark Error: Method must be provided");
        }
        
        // Validate iterations is greater than 0
        if (benchmarkRequest.Iterations <= 0)
        {
            throw new ArgumentException("Run Benchmark Error: Iterations must be greater than 0");
        }
        
        // Business Rule: Limit iterations to 50 to prevent abuse
        if (benchmarkRequest.Iterations > 50)
        {
            throw new ArgumentException("Run Benchmark Error: Iterations must be less than or equal to 50 to prevent abuse");
        }

        BenchmarkResult result = new();
        Stopwatch stopwatch = new();

        for (int i = 0; i < benchmarkRequest.Iterations; i++)
        {
            stopwatch.Restart();
            await benchmarkRequest.Method();
            stopwatch.Stop();
            
            result.ExecutionTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
            
            // if a progress object was provided, report progress
            benchmarkRequest.Progress?.Report((i + 1.0f)/ benchmarkRequest.Iterations);
        }
        
        // Set the Median execution time. The rest of the properties are auto calculated
        result.MedianTime = result.ExecutionTimes.Median();
        
        return result;
    }
}