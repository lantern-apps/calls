using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.MultipleParameterTest;

namespace Calls.Test;

[IncludeType(typeof(MultipleParametersTestHandler))]
public partial class MultipleParameterTestCall : Call { }

[TestClass]
public class MultipleParameterTest
{
    private readonly ICall _call;

    public MultipleParameterTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<MultipleParametersTestHandler>()
            .AddSingleton<ICall, MultipleParameterTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void Invoke()
    {
        Assert.AreEqual(0, _call.Invoke<int>(null));
        Assert.AreEqual(1, _call.Invoke<int>(null, 1));
        Assert.AreEqual(2, _call.Invoke<int>(null, 1, 2));
        Assert.AreEqual(3, _call.Invoke<int>(null, 1, 2, 3));
        Assert.AreEqual(4, _call.Invoke<int>(null, 1, 2, 3, 4));
        Assert.AreEqual(5, _call.Invoke<int>(null, 1, 2, 3, 4, 5));
        Assert.AreEqual(6, _call.Invoke<int>(null, 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public void Invoke_WithRoutingKey()
    {
        Assert.AreEqual(0, _call.Invoke<int>("0"));
        Assert.AreEqual(1, _call.Invoke<int>("1", 1));
        Assert.AreEqual(2, _call.Invoke<int>("2", 1, 2));
        Assert.AreEqual(3, _call.Invoke<int>("3", 1, 2, 3));
        Assert.AreEqual(4, _call.Invoke<int>("4", 1, 2, 3, 4));
        Assert.AreEqual(5, _call.Invoke<int>("5", 1, 2, 3, 4, 5));
        Assert.AreEqual(6, _call.Invoke<int>("6", 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public async Task AsyncInvoke()
    {
        Assert.AreEqual(10, await _call.InvokeAsync<int>(null));
        Assert.AreEqual(11, await _call.InvokeAsync<int>(null, 1));
        Assert.AreEqual(12, await _call.InvokeAsync<int>(null, 1, 2));
        Assert.AreEqual(13, await _call.InvokeAsync<int>(null, 1, 2, 3));
        Assert.AreEqual(14, await _call.InvokeAsync<int>(null, 1, 2, 3, 4));
        Assert.AreEqual(15, await _call.InvokeAsync<int>(null, 1, 2, 3, 4, 5));
        Assert.AreEqual(16, await _call.InvokeAsync<int>(null, 1, 2, 3, 4, 5, 6));
    }

    [TestMethod]
    public async Task AsyncInvoke_WithRoutingKey()
    {
        Assert.AreEqual(10, await _call.InvokeAsync<int>("0"));
        Assert.AreEqual(11, await _call.InvokeAsync<int>("1", 1));
        Assert.AreEqual(12, await _call.InvokeAsync<int>("2", 1, 2));
        Assert.AreEqual(13, await _call.InvokeAsync<int>("3", 1, 2, 3));
        Assert.AreEqual(14, await _call.InvokeAsync<int>("4", 1, 2, 3, 4));
        Assert.AreEqual(15, await _call.InvokeAsync<int>("5", 1, 2, 3, 4, 5));
        Assert.AreEqual(16, await _call.InvokeAsync<int>("6", 1, 2, 3, 4, 5, 6));
    }

    public class MultipleParametersTestHandler
    {
        [Callable] public int MultipleParametersTest() => 0;
        [Callable] public int MultipleParametersTest(int a) => 1;
        [Callable] public int MultipleParametersTest(int a, int b) => 2;
        [Callable] public int MultipleParametersTest(int a, int b, int c) => 3;
        [Callable] public int MultipleParametersTest(int a, int b, int c, int d) => 4;
        [Callable] public int MultipleParametersTest(int a, int b, int c, int d, int e) => 5;
        [Callable] public int MultipleParametersTest(int a, int b, int c, int d, int e, int f) => 6;

        [Callable] public Task<int> MultipleParametersTestAsync() => Task.FromResult(10);
        [Callable] public Task<int> MultipleParametersTestAsync(int a) => Task.FromResult(11);
        [Callable] public Task<int> MultipleParametersTestAsync(int a, int b) => Task.FromResult(12);
        [Callable] public Task<int> MultipleParametersTestAsync(int a, int b, int c) => Task.FromResult(13);
        [Callable] public Task<int> MultipleParametersTestAsync(int a, int b, int c, int d) => Task.FromResult(14);
        [Callable] public Task<int> MultipleParametersTestAsync(int a, int b, int c, int d, int e) => Task.FromResult(15);
        [Callable] public Task<int> MultipleParametersTestAsync(int a, int b, int c, int d, int e, int f) => Task.FromResult(16);

        [Callable("0")] public int NamedMethodMultipleParametersTest() => 0;
        [Callable("1")] public int NamedMethodMultipleParametersTest(int a) => 1;
        [Callable("2")] public int NamedMethodMultipleParametersTest(int a, int b) => 2;
        [Callable("3")] public int NamedMethodMultipleParametersTest(int a, int b, int c) => 3;
        [Callable("4")] public int NamedMethodMultipleParametersTest(int a, int b, int c, int d) => 4;
        [Callable("5")] public int NamedMethodMultipleParametersTest(int a, int b, int c, int d, int e) => 5;
        [Callable("6")] public int NamedMethodMultipleParametersTest(int a, int b, int c, int d, int e, int f) => 6;

        [Callable("0")] public Task<int> NamedMethodMultipleParametersTestAsync() => Task.FromResult(10);
        [Callable("1")] public Task<int> NamedMethodMultipleParametersTestAsync(int a) => Task.FromResult(11);
        [Callable("2")] public Task<int> NamedMethodMultipleParametersTestAsync(int a, int b) => Task.FromResult(12);
        [Callable("3")] public Task<int> NamedMethodMultipleParametersTestAsync(int a, int b, int c) => Task.FromResult(13);
        [Callable("4")] public Task<int> NamedMethodMultipleParametersTestAsync(int a, int b, int c, int d) => Task.FromResult(14);
        [Callable("5")] public Task<int> NamedMethodMultipleParametersTestAsync(int a, int b, int c, int d, int e) => Task.FromResult(15);
        [Callable("6")] public Task<int> NamedMethodMultipleParametersTestAsync(int a, int b, int c, int d, int e, int f) => Task.FromResult(16);

    }
}

