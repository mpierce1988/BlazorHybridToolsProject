namespace ResultSetInterpreter.Models.Benchmark;

public class BenchmarkResult
{
    public double AverageTime => ExecutionTimes.Count == 0 ? 0 : ExecutionTimes.Average();

    public double MedianTime { get; set; }
    public double MinTime => ExecutionTimes.Count == 0 ? 0 : ExecutionTimes.Min();
    public double MaxTime => ExecutionTimes.Count == 0 ? 0 : ExecutionTimes.Max();
    public List<double> ExecutionTimes { get; set; } = new();
}