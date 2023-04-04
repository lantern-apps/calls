namespace Calls.SourceGeneration.Test;

[TestClass]
public class SourceGenerateTest
{
    [TestMethod]
    public void Nested_Interface()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public class RootType { 
    public interface NestedType{
        [Callable] public void Handle(string package); 
    }
}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodDeclaringType("global::Test.RootType.NestedType");
        AssertHelper.AssertMethod("Handle");
    }

    [TestMethod]
    public void Interface_NullKey_Void()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable] public void Handle(string package); }";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle");
    }

    [TestMethod]
    public void Interface_WithKey_Void()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable(""key"")] public void Handle(string package); }";
        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", "key");
    }

    [TestMethod]
    public void Interface_WithMethodName_Void()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable(true)] public void Method(string package); }";
        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Method", "Method");
    }


    [TestMethod]
    public void Interface_WithKey_ResultType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable(""key"")] public string Handle(string package); }";
        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", "key", "string");
    }

    [TestMethod]
    public void Interface_WithKey_ResultType_Awaitable()
    {
        var source = @"using Calls;using System.Threading.Tasks; namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable(""key"")] public Task<string> Handle(string package); }";
        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", "key", "string", true);
    }

    [TestMethod]
    public void Interface_WithKey_Awaitable()
    {
        var source = @"using Calls;using System.Threading.Tasks; namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable(""key"")] public Task Handle(string package); }";
        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle", "key", null, true);
    }
}