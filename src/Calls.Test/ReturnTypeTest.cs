using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.ReturnTypeTest;

namespace Calls.Test;

[IncludeType(typeof(ReturnTypeTestHandler))]
public partial class ReturnTypeTestCall : Call { }

[TestClass]
public class ReturnTypeTest
{
    private readonly ICall _call;

    public ReturnTypeTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<ReturnTypeTestHandler>()
            .AddSingleton<ICall, ReturnTypeTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void Nullable_ReturnNull() => Assert.IsNull(_call.Invoke<bool?>("Nullable"));

    [TestMethod]
    public void Nullable_ReturnValue() => Assert.IsTrue(_call.Invoke<bool?>("NullableValue"));

    public class ReturnTypeTestHandler
    {
        [Callable("Nullable")] public bool? Nullable() => null;
        [Callable("NullableValue")] public bool? NullableValue() => true;
    }
}
