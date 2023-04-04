using Microsoft.Extensions.DependencyInjection;
using Calls;
using static Calls.Test.ParameterCompatibleTest;

namespace Calls.Test;

[IncludeType(typeof(ParameterCompatibleTestHandler))]
public partial class ParameterCompatibleTestCall : Call { }

[TestClass]
public class ParameterCompatibleTest
{
    private readonly ICall _call;

    public ParameterCompatibleTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<ParameterCompatibleTestHandler>()
            .AddSingleton<ICall, ParameterCompatibleTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void ExactMatch() => Assert.AreEqual(1, _call.Invoke<int>(null, new B()));

    [TestMethod]
    public void Compatible() => Assert.AreEqual(0, _call.Invoke<int>(null, new C()));

    public class ParameterCompatibleTestHandler
    {
        [Callable] public int ParameterCompatibleTest(A a) => 0;
        [Callable] public int ParameterCompatibleTest(B b) => 1;
    }

    public class A { }
    public class B : A { }
    public class C : A { }
}
