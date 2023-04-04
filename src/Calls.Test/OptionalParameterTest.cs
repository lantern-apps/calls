using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.OptionalParameterTest;

namespace Calls.Test;

[IncludeType(typeof(OptionalParameterTestHandler))]
public partial class OptionalParameterTestCall : Call { }

[TestClass]
public class OptionalParameterTest
{
    private readonly ICall _call;

    public OptionalParameterTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<OptionalParameterTestHandler>()
            .AddSingleton<ICall, OptionalParameterTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void Invoke()
    {
        Assert.AreEqual(6, _call.Invoke<int>(null));
        Assert.AreEqual(6, _call.Invoke<int>(null, 1));
        Assert.AreEqual(7, _call.Invoke<int>(null, 1, 2));
        Assert.AreEqual(9, _call.Invoke<int>(null, 1, 2, 3));
        Assert.AreEqual(12, _call.Invoke<int>(null, 1, 2, 3, 4));
        Assert.AreEqual(16, _call.Invoke<int>(null, 1, 2, 3, 4, 5));
        Assert.AreEqual(21, _call.Invoke<int>(null, 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public void Invoke_WithRoutingKey()
    {
        Assert.AreEqual(6, _call.Invoke<int>("a"));
        Assert.AreEqual(6, _call.Invoke<int>("a", 1));
        Assert.AreEqual(7, _call.Invoke<int>("a", 1, 2));
        Assert.AreEqual(9, _call.Invoke<int>("a", 1, 2, 3));
        Assert.AreEqual(12, _call.Invoke<int>("a", 1, 2, 3, 4));
        Assert.AreEqual(16, _call.Invoke<int>("a", 1, 2, 3, 4, 5));
        Assert.AreEqual(21, _call.Invoke<int>("a", 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public async Task AsyncInvoke()
    {
        Assert.AreEqual(6, await _call.InvokeAsync<int>(null));
        Assert.AreEqual(6, await _call.InvokeAsync<int>(null, 1));
        Assert.AreEqual(7, await _call.InvokeAsync<int>(null, 1, 2));
        Assert.AreEqual(9, await _call.InvokeAsync<int>(null, 1, 2, 3));
        Assert.AreEqual(12, await _call.InvokeAsync<int>(null, 1, 2, 3, 4));
        Assert.AreEqual(16, await _call.InvokeAsync<int>(null, 1, 2, 3, 4, 5));
        Assert.AreEqual(21, await _call.InvokeAsync<int>(null, 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public async Task AsyncInvoke_WithRoutingKey()
    {
        Assert.AreEqual(6, await _call.InvokeAsync<int>("a"));
        Assert.AreEqual(6, await _call.InvokeAsync<int>("a", 1));
        Assert.AreEqual(7, await _call.InvokeAsync<int>("a", 1, 2));
        Assert.AreEqual(9, await _call.InvokeAsync<int>("a", 1, 2, 3));
        Assert.AreEqual(12, await _call.InvokeAsync<int>("a", 1, 2, 3, 4));
        Assert.AreEqual(16, await _call.InvokeAsync<int>("a", 1, 2, 3, 4, 5));
        Assert.AreEqual(21, await _call.InvokeAsync<int>("a", 1, 2, 3, 4, 5, 6));
    }

    public class OptionalParameterTestHandler
    {
        [Callable] public int DefaultParameterTest(int a = 1, int b = 1, int c = 1, int d = 1, int e = 1, int f = 1) => a + b + c + d + e + f;
        [Callable] public Task<int> DefaultParameterTestAsync(int a = 1, int b = 1, int c = 1, int d = 1, int e = 1, int f = 1) => Task.FromResult(a + b + c + d + e + f);
        [Callable("a")] public int NamedMethodDefaultParameterTest(int a = 1, int b = 1, int c = 1, int d = 1, int e = 1, int f = 1) => a + b + c + d + e + f;
        [Callable("a")] public Task<int> NamedMethodDefaultParameterTestAsync(int a = 1, int b = 1, int c = 1, int d = 1, int e = 1, int f = 1) => Task.FromResult(a + b + c + d + e + f);
    }
}

