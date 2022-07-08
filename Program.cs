var rnd = new Random();
using var srv = new Server(HttpHandler);

while (true)
{
    Console.WriteLine("RunTasks for Wait...");
    var tasks = RunTasks();
    Console.WriteLine("WaitAny...");
    Task.WaitAny(tasks);
    Console.WriteLine("WaitAll...");
    Task.WaitAll(tasks);

    Console.WriteLine("RunTasks for When...");
    tasks = RunTasks();
    Console.WriteLine("WhenAny...");
    await Task.WhenAny(tasks);
    Console.WriteLine("WhenAll...");
    await Task.WhenAll(tasks);
}

Task[] RunTasks()
{
    var tasks = new List<Task>();

    tasks.Add(Task.Run(
        () => Async().Wait()
    ));
    tasks.Add(Task.Run(
        async () => await Async()
    ));
    tasks.Add(Task.Run(
        () => TAsync().Result
    ));
    tasks.Add(Task.Run(
        async () => await TAsync()
    ));
    tasks.Add(Task.Run(
        () => ValueTAsync().Result
    ));
    tasks.Add(Task.Run(
        async () => await ValueTAsync()
    ));
    tasks.Add(
        ValueTAsync().AsTask()
    );

    tasks.Add(Task.Run(
        () => HttpRequestAsync().Wait()
    ));
    tasks.Add(Task.Run(
        async () => await HttpRequestAsync()
    ));
    tasks.Add(HttpRequestAsync());

    return tasks.ToArray();
}

Task Async() => Task.Run(Sort);

Task<int> TAsync() => Task.Run<int>(() => { Sort(); return 0; });
async ValueTask<int> ValueTAsync() => await TAsync();

void Sort()
{
    var list = new List<int>();
    var count = 100000000;
    var len = Environment.GetEnvironmentVariable("SORT_LEN");
    if (!string.IsNullOrEmpty(len))
    {
        count = int.Parse(len);
    }
    
    for (int i = 0; i < count; i++)
    {
        var val = rnd.Next();
        list.Add(val);
    }

    using (new Transaction("Sort"))
    {
        list.Sort();
    }
}

void HttpHandler(HttpListenerContext ctx)
{
    using (new Transaction("Http.Handler"))
    {
        using var evnt = new AutoResetEvent(false);
        Task.Run(() =>
        {
            Thread.Sleep(TimeSpan.FromSeconds(20));
            evnt.Set();
        });
        evnt.WaitOne();

        ctx.Response.ContentType = "text/plain";
        var buffer = System.Text.Encoding.UTF8.GetBytes("OK");
        ctx.Response.ContentLength64 = buffer.LongLength;
        ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
        ctx.Response.Close();
    }
}

async Task<string> HttpRequestAsync()
{
    using (new Transaction("Http.Request"))
    {
        var response = await new HttpClient().GetAsync(srv.Url);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}
