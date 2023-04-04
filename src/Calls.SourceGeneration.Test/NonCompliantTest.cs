namespace Calls.SourceGeneration.Test;

[TestClass]
public class NonCompliantTest
{
    [TestMethod]
    public void MissPartialKeyword()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public class CallTest : Call{}
public interface IHandler { [Callable]void Handle(string a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void MissBaseTypeKeyword()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest {}
public interface IHandler { [Callable]void Handle(string a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public  void AbstractType()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public abstract partial class CallTest : Call {}
public interface IHandler { [Callable]void Handle(string a);} ";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void GenericCallerType()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest<T> : Call {}
public interface IHandler { [Callable]void Handle(string a);} ";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void GenericHandlerType()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest : Call {}
public class Handler<T> { [Callable]void Handle(string a){}} ";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void StaticType()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public static partial class CallTest : Call {}
public interface IHandler { [Callable]void Handle(string a);} ";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }


    [TestMethod]
    public void RefParameter()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest : Call {}
public interface IHandler { [Callable]void Handle(ref int a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void OutParameter()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest : Call {}
public interface IHandler { [Callable]void Handle(out int a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void GenericMethod()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest : Call{}
public interface IHandler { [Callable]void Handle<T>(T a);}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void NonPublicMethod()
    {
        var source = @"using Calls; 
namespace Test;
[CallableCurrentAssembly]public partial class CallTest : Call {}
public class Handler { [Callable]void Handle(int a){}}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }

    [TestMethod]
    public void NestedCallerType()
    {
        var source = @"using Calls; 
namespace Test;
public class Container { [CallableCurrentAssembly] public partial class CallTest : Call {} }
public class Handler { [Callable]public void Handle(int a){}}";

        var result = CSharpGeneratorHelper.Generate(source);
        AssertHelper.AsssetNoGenerated(result);
    }
}

