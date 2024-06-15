using ResultSetInterpreter.Models.Benchmark;

namespace ResultSetInterpreter.Services.Interfaces;

public interface IBenchmarkService
{
    public Task<BenchmarkResult> RunBenchmarkAsync(BenchmarkRequest benchmarkRequest);
}