using Calls.SourceGeneration.Test.Library;

namespace Calls.SourceGeneration.Test;

[TestClass]
public class SpecifyIncludeAssemblyTest
{
    [TestMethod]
    public void IncludeCurrentAssembly()
    {
        var source = @"using Calls; 
namespace Test
{
    [IncludeAssembly(typeof(Test2.IHandler))] public partial class CallTest : Call{}
}
namespace Test2
{
    public interface IHandler { [Callable] public void Handle(string package); }
}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle");
    }

    [TestMethod]
    public void IncludeDefaultAssembly()
    {
        var source = @"using Calls; 
namespace Test
{
    [IncludeAssembly] public partial class CallTest : Call{}
}
namespace Test2
{
    public interface IHandler { [Callable] public void Handle(string package); }
}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle");
    }

    [TestMethod]
    public void IncludeAssemblyWithCurrent()
    {
        var source = @"using Calls; 
namespace Test
{
    [IncludeAssembly] 
    [IncludeAssembly(typeof(Calls.SourceGeneration.Test.Library.ClassHandler2))] 
    public partial class CallTest : Call{}
}
namespace Test2
{
    public interface IHandler { [Callable] public void Handle3(string s); }
}";

        var result = CSharpGeneratorHelper.Generate(source, typeof(ClassHandler).Assembly);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodCount(3);
    }


    [TestMethod]
    public void IncludeAssembly()
    {
        var source = @"using Calls; 
namespace Test
{
[IncludeAssembly(typeof(Calls.SourceGeneration.Test.Library.ClassHandler2))] 
public partial class CallTest : Call{}
}";

        var result = CSharpGeneratorHelper.Generate(source, typeof(ClassHandler).Assembly);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodCount(2);
    }
}