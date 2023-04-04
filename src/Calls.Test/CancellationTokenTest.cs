using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.CancellationTokenTest;

namespace Calls.Test;

[IncludeType(typeof(CancellationTokenTestHandler))]
public partial class CancellationTokenTestCall : Call { }

[TestClass]
public class CancellationTokenTest
{
    private readonly ICall _call;

    public CancellationTokenTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<CancellationTokenTestHandler>()
            .AddSingleton<ICall, CancellationTokenTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void MissCancallationToken()
    {
        Assert.AreEqual(1, _call.Invoke<int>("CancellationToken"));
        Assert.AreEqual(1, _call.Invoke<int>("CancellationToken", 1));
    }

    [TestMethod]
    public void MissCancallationTokenWithDefaultValue()
    {
        Assert.AreEqual(1, _call.Invoke<int>("CancellationTokenWithDefaultValue"));
        Assert.AreEqual(1, _call.Invoke<int>("CancellationTokenWithDefaultValue", 1));
    }

    [TestMethod]
    public void ExtraCancallationToken()
    {
        Assert.AreEqual(1, _call.Invoke<int>("NonCancellationToken", CancellationToken.None));
        Assert.AreEqual(1, _call.Invoke<int>("NonCancellationToken", 1, CancellationToken.None));
    }

    [TestMethod]
    public void MiddleCancallationToken()
    {
        Assert.ThrowsException<InvalidOperationException>(() => _call.Invoke<int>("CancellationTokenInMiddle", 1));
        Assert.ThrowsException<InvalidOperationException>(() => _call.Invoke<int>("CancellationTokenInMiddleWithDefaultValue", 1));
        Assert.AreEqual(1, _call.Invoke<int>("CancellationTokenInMiddleWithDefaultValue2", 1, CancellationToken.None));
    }

    public class CancellationTokenTestHandler
    {
        [Callable("NonCancellationToken")] public int CancellationTokenTest() => 1;
        [Callable("CancellationToken")] public int CancellationTokenTest(CancellationToken cancellationToken) => 1;
        [Callable("CancellationTokenWithDefaultValue")] public int CancellationTokenWithDefaultValueTest(CancellationToken cancellationToken = default) => 1;

        [Callable("NonCancellationToken")] public int CancellationTokenTest(int a) => 1;
        [Callable("CancellationToken")] public int CancellationTokenTest(int a, CancellationToken cancellationToken) => 1;
        [Callable("CancellationTokenWithDefaultValue")] public int CancellationTokenWithDefaultValueTest(int a, CancellationToken cancellationToken = default) => 1;

        [Callable("CancellationTokenInMiddle")] public int CancellationTokenTest(int a, CancellationToken cancellationToken, int b) => 1;
        [Callable("CancellationTokenInMiddleWithDefaultValue")] public int CancellationTokenInMiddleWithDefaultValueTest(int a, CancellationToken cancellationToken, int b = 3) => 1;
        [Callable("CancellationTokenInMiddleWithDefaultValue2")] public int CancellationTokenInMiddleWithDefaultValue2Test(int a, CancellationToken cancellationToken = default, int b = 3) => 1;

    }

    public class A { }
    public class B : A { }
    public class C : A { }
}
