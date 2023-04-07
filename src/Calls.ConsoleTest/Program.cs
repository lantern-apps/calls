using Calls;
using Microsoft.Extensions.DependencyInjection;

var call = new ServiceCollection()
    .AddSingleton<ICall, MethodCall>()
    .AddScoped<Handler>()
    .BuildServiceProvider()
    .GetRequiredService<ICall>();

Console.WriteLine(call.Invoke<int>(null, 1, 2));
Console.ReadLine();

[IncludeAssembly]
public partial class MethodCall : Call { }

public class Handler
{
    [Callable]
    public int Add(int a, int b) => a + b;
}