namespace ResultSetInterpreter.Models.Benchmark;

public class BenchmarkRequest
{
    public Func<Task>? Method { get; set; }
    public int Iterations { get; set; }
    public IProgress<double>? Progress { get; set; }
}