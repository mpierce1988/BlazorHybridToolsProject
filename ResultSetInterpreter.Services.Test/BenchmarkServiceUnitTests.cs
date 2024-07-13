using ResultSetInterpreter.Models.Benchmark;

namespace ResultSetInterpreter.Services.Test;

public class BenchmarkServiceUnitTests
{
    private readonly IBenchmarkService _benchmarkService = new BenchmarkService();

    [Fact]
    public async Task RunForNumberOfIterations_ShouldReturnSameNumberResults()
    {
        // arrange
        int numIterations = 10;
        BenchmarkRequest request = new()
        {
            Method = () => Task.FromResult(0),
            Iterations = numIterations
        };
        
        // act
        BenchmarkResult result = await _benchmarkService.RunBenchmarkAsync(request);
        
        // assert
        Assert.Equal(numIterations, result.ExecutionTimes.Count);
    }

    [Fact]
    public async Task InvalidNumberOfIterations_ShouldThrowArgumentExceptionError()
    {
        // arrange
        int numIterations = 0;
        BenchmarkRequest request = new()
        {
            Method = () => Task.FromResult(0),
            Iterations = numIterations
        };
        
        // in order to test exceptions, we need to act and assert in the same line
        await Assert.ThrowsAsync<ArgumentException>(() => _benchmarkService.RunBenchmarkAsync(request));
    }
    
    [Fact]
    public async Task IterationsGreaterThan50_ShouldThrowArgumentExceptionError()
    {
        // arrange
        int numIterations = 51;
        BenchmarkRequest request = new()
        {
            Method = () => Task.FromResult(0),
            Iterations = numIterations
        };
        
        // in order to test exceptions, we need to act and assert in the same line
        await Assert.ThrowsAsync<ArgumentException>(() => _benchmarkService.RunBenchmarkAsync(request));
    }
    
    [Fact]
    public async Task MethodNotProvided_ShouldThrowArgumentExceptionError()
    {
        // arrange
        BenchmarkRequest request = new()
        {
            Method = null,
            Iterations = 10
        };
        
        // in order to test exceptions, we need to act and assert in the same line
        await Assert.ThrowsAsync<ArgumentException>(() => _benchmarkService.RunBenchmarkAsync(request));
    }
}