using Microsoft.Extensions.DependencyInjection;
using static Calls.Test.ParameterTypeTest;

namespace Calls.Test;

[IncludeType(typeof(ParameterTypeTestHandler))]
public partial class PrameterTypeTestCall : Call { }

[TestClass]
public class ParameterTypeTest
{
    private readonly ICall _call;

    public ParameterTypeTest()
    {
        _call = new ServiceCollection()
            .AddSingleton<ParameterTypeTestHandler>()
            .AddSingleton<ICall, PrameterTypeTestCall>()
            .BuildServiceProvider()
            .GetRequiredService<ICall>();
    }

    [TestMethod]
    public void Generic()
    {
        Assert.AreEqual(1, _call.Invoke<GenericType<int>>("Generic", new GenericType<int>(1))!.Value);
    }

    [TestMethod]
    public void Nullable()
    {
        Assert.AreEqual(true, _call.Invoke<bool?>("Nullable", true));
        Assert.AreEqual(null, _call.Invoke<bool?>("Nullable", new object?[] { null }));
    }

    [TestMethod] public void Nested() => Assert.AreEqual(1, _call.Invoke<NestedType.NestedType2>("Nested", new NestedType.NestedType2(1))!.Value);
    [TestMethod] public void Enum() => Assert.AreEqual(EnumA.B, _call.Invoke<EnumA>("Enum", EnumA.B));
    [TestMethod] public void Array() => Assert.AreEqual(1, _call.Invoke<int[]>("Array", new int[] { 1 })![0]);

    public class ParameterTypeTestHandler
    {
        [Callable("Generic")] public GenericType<int> Generic(GenericType<int> a) => a;
        [Callable("Nullable")] public bool? Nullable(bool? a) => a;
        [Callable("Nested")] public NestedType.NestedType2 Nested(NestedType.NestedType2 a) => a;
        [Callable("Array")] public int[] Array(int[] a) => a;
        [Callable("Enum")] public EnumA Enum(EnumA a) => a;
    }

    public enum EnumA
    {
        A,
        B,
    }

    public class GenericType<T>
    {
        public GenericType(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }

    public class NestedType
    {
        public class NestedType2
        {
            public NestedType2(int value)
            {
                Value = value;
            }

            public int Value { get; set; }
        }
    }


}
