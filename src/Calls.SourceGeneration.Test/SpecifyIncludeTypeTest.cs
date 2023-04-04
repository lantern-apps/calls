using Calls.SourceGeneration.Test.Library;

namespace Calls.SourceGeneration.Test;

[TestClass]
public class SpecifyIncludeTypeTest
{
    [TestMethod]
    public void IncludeCurrentAssemblyType()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeType(typeof(IHandler))] public partial class CallTest : Call{}
public interface IHandler { [Callable] public void Handle(string package); }";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle");
    }

    [TestMethod]
    public void IncludeAssemblyType()
    {
        var source = @"using Calls; 
namespace Test
{
[IncludeType(typeof(Calls.SourceGeneration.Test.Library.ClassHandler))] 
public partial class CallTest : Call{}
}";

        var result = CSharpGeneratorHelper.Generate(source, typeof(ClassHandler).Assembly);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethod("Handle");
    }

}
