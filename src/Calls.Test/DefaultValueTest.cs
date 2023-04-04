using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.DefaultValueTest;
using static Calls.Test.DefaultValueTest.DefaultValueTestHandler;

namespace Calls.Test;

[IncludeType(typeof(DefaultValueTestHandler))]
public partial class DefaultValueTestCall : Call { }

[TestClass]
public class DefaultValueTest
{
    private readonly ICall _call;
    public DefaultValueTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<DefaultValueTestHandler>()
            .AddSingleton<ICall, DefaultValueTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]  public void Const() => Assert.AreEqual(1, _call.Invoke<int>("Const"));
    [TestMethod] public void Boolean() => Assert.AreEqual(true, _call.Invoke<bool>("Boolean"));
    [TestMethod] public void String() => Assert.AreEqual("a", _call.Invoke<string>("String"));
    [TestMethod] public void Enum() => Assert.AreEqual(EnumA.B, _call.Invoke<EnumA>("Enum"));
    [TestMethod] public void Char() => Assert.AreEqual('a', _call.Invoke<char>("Char"));

    public class DefaultValueTestHandler
    {
        private const int A = 1;

        public enum EnumA
        {
            A,
            B,
        }

        [Callable("Const")] public int Const(int a = A) => a;
        [Callable("Boolean")] public bool Boolean(bool a = true) => a;
        [Callable("String")] public string String(string a = "a") => a;
        [Callable("Enum")] public EnumA Enum(EnumA a = EnumA.B) => a;
        [Callable("Char")] public char Char(char a = 'a') => a;
    }

}

