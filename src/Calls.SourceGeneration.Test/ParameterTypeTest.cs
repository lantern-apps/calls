namespace Calls.SourceGeneration.Test;

[TestClass]
public class ParameterTypeTest
{
    [TestMethod]
    public void Nullable()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly] public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle(int? a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("int?");
    }
}

