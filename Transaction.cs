public class Transaction : IDisposable
{
    private readonly string name;
    private readonly OpenTracing.IScope span;
    private readonly Stopwatch stopwatch;
    private readonly string id;

    public Transaction(string name)
    {
        this.name = name;
        this.id = Guid.NewGuid().ToString();
        
        Console.WriteLine($"{name} {id} started");
        
        this.span = OpenTracing.Util.GlobalTracer.Instance.BuildSpan(name).StartActive();
        this.stopwatch = Stopwatch.StartNew();
    }

    public void Dispose()
    {
        stopwatch.Stop();
        span.Span.Finish();
        
        Console.WriteLine($"{name} {id} finished after {stopwatch.Elapsed}"); 
    }
}