namespace Calls.SourceGeneration.Test;

[TestClass]
public class ReturnTypeTest
{
    [TestMethod]
    public void Return_ValueType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]int Handle();}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "int", false);
    }

    [TestMethod]
    public void Return_NullableType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]int? Handle();}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "int?", false);
    }

    [TestMethod]
    public void Return_ArrayType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]int[] Handle();}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "int[]", false);
    }

    [TestMethod]
    public void Return_GenericType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]GenericType<int> Handle();}
public class GenericType<T>{}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "global::Test.GenericType<int>", false);
    }

    [TestMethod]
    public void Return_ObjectType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]object Handle();}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "object", false);
    }

    [TestMethod]
    public void Return_NestedType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call {}
public class Handler { 
    [Callable]public NestedType Handle() { return null; }
    public class NestedType { }
}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", null, "global::Test.Handler.NestedType", false);
    }
}

