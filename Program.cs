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
        () => BogoAsync().Wait()
    ));
    tasks.Add(Task.Run(
        async () => await BogoAsync()
    ));
    tasks.Add(Task.Run(
        () => BogoTAsync().Result
    ));
    tasks.Add(Task.Run(
        async () => await BogoTAsync()
    ));
    tasks.Add(Task.Run(
        () => BogoValueTAsync().Result
    ));
    tasks.Add(Task.Run(
        async () => await BogoValueTAsync()
    ));
    tasks.Add(
        BogoValueTAsync().AsTask()
    );

    return tasks.ToArray();
}

Task BogoAsync() => Task.Run(BogoSort);

Task<int> BogoTAsync() => Task.Run<int>(() => { BogoSort(); return 0; });
async ValueTask<int>  BogoValueTAsync() => await BogoTAsync();

void BogoSort()
{
    var rnd = new Random();
    var list = new List<int>();
    var count = 11;
    for (int i = 0; i < count; i++)
    {
        var val = rnd.Next();
        list.Add(val);
    }

    using (new Transaction("Bogo.Sort"))
    {
        Bogo.Sort(list);
    }
}






