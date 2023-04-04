namespace Calls.SourceGeneration.Test;

[TestClass]
public class NamespaceSourceGenerateTest
{
    [TestMethod]
    public void GlobalNamespace()
    {
        var source = @"using Calls; 
[IncludeAssembly]
public partial class CallTest : Call{}
public interface IHandler { [Callable] public void Handle(string package); }";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertType(null, "CallTest");
        AssertHelper.AssertMethod("Handle");
    }

}

