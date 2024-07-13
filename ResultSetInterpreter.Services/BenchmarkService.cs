using System.Diagnostics;
using ResultSetInterpreter.Models.Benchmark;
using ResultSetInterpreter.Services.Interfaces;
using ResultSetInterpreter.Services.Utilities;

namespace ResultSetIntrepreter.Services;

public class BenchmarkService : IBenchmarkService
{
    #region Public Methods
    
    /// <summary>Runs a benchmark on the provided method</summary>
    /// <param name="benchmarkRequest">The benchmark request object containing the method to benchmark and the
    /// number of iterations to run</param>
    /// <returns>The benchmark result object containing the execution times and other statistics</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the method is null or the number of iterations is less than or equal to 0
    /// </exception>
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
            result.ExecutionTimes.Add(await RunIteration(benchmarkRequest.Method, stopwatch));

            // if a progress object was provided, report progress
            benchmarkRequest.Progress?.Report((i + 1.0f)/ benchmarkRequest.Iterations);
        }
        
        // Set the Median execution time. The rest of the properties are auto calculated
        result.MedianTime = result.ExecutionTimes.Median();
        
        return result;
    }
    
    #endregion
    
    #region Private Methods

    /// <summary>Runs a single iteration of the benchmark</summary>
    /// <param name="methodToTest">The benchmark request object containing the method to benchmark</param>
    /// <param name="stopwatch">The stopwatch object to use for timing the method</param>
    /// <returns>The execution time of the method</returns> 
    private async Task<double> RunIteration(Func<Task>? methodToTest, Stopwatch stopwatch)
    {
        stopwatch.Restart();
        await methodToTest!(); // Method has already been validated
        stopwatch.Stop();
        
        return stopwatch.Elapsed.TotalMilliseconds;
    }
    
    #endregion
}