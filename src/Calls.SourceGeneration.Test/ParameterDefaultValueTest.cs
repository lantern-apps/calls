namespace Calls.SourceGeneration.Test;

[TestClass]
public class ParameterDefaultValueTest
{
    [TestMethod]
    public void Int_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle(int a = 1);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("int", "1");
    }

    [TestMethod]
    public void Bool_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle(bool a = true);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("bool", "true");
    }

    [TestMethod]
    public void String_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle(string a = ""abc"");}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("string", "\"abc\"");
    }

    [TestMethod]
    public void Null_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle(string a = null);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("string", "null");
    }


    [TestMethod]
    public void Const_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public interface IHandler { 
public const int A = 2;
[Callable]void Handle(int a = A);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("int", "2");
    }

    [TestMethod]
    public void Enum_Default_Value()
    {
        var source = @"using Calls; 
namespace Test;
[IncludeAssembly]public partial class CallTest : Call{}
public enum E {A,B,C}
public interface IHandler { 
[Callable]void Handle(E a = E.C);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetDiagnostics(result);
        AssertHelper.AssertMethodParameter("global::Test.E", "global::Test.E.C");
    }
}

