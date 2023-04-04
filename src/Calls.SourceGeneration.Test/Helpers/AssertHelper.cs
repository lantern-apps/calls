using Microsoft.CodeAnalysis;

namespace Calls.SourceGeneration.Test;

public static class AssertHelper
{
    public static void AssertType(
        string? @namespace,
        string? name)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration);
        Assert.AreEqual(@namespace, CallsSourceGenerator.LastGeneration.Namespace);
        Assert.AreEqual(name, CallsSourceGenerator.LastGeneration.Name);
    }

    public static void AssertMethodDeclaringType(string type)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration?.Methods);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods.Length);
        Assert.AreEqual(type, CallsSourceGenerator.LastGeneration.Methods[0].Type);
    }

    public static void AssertMethodParameter(string type)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration?.Methods);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods.Length);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods[0].Parameters.Length);

        var parameter = CallsSourceGenerator.LastGeneration.Methods[0].Parameters[0];
        Assert.AreEqual(type, parameter.Type);
        Assert.IsFalse(parameter.HasDefaultValue);
    }

    public static void AssertMethodParameter(string type, string defaultValue)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration?.Methods);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods.Length);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods[0].Parameters.Length);

        var parameter = CallsSourceGenerator.LastGeneration.Methods[0].Parameters[0];
        Assert.AreEqual(type, parameter.Type);
        Assert.IsTrue(parameter.HasDefaultValue);
        Assert.AreEqual(defaultValue, parameter.DefaultValue);
    }

    public static void AssertMethodCount(int count)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration?.Methods);
        Assert.AreEqual(count, CallsSourceGenerator.LastGeneration.Methods.Length);
    }

    public static void AssertMethod(
        string methodName,
        string? routingKey = null,
        string? resultType = null,
        bool awaitable = false)
    {
        Assert.IsNotNull(CallsSourceGenerator.LastGeneration?.Methods);
        Assert.AreEqual(1, CallsSourceGenerator.LastGeneration.Methods.Length);
        Assert.AreEqual(routingKey, CallsSourceGenerator.LastGeneration.Methods[0].RoutingKey);
        Assert.AreEqual(methodName, CallsSourceGenerator.LastGeneration.Methods[0].MethodName);
        Assert.AreEqual(resultType, CallsSourceGenerator.LastGeneration.Methods[0].ReturnType);
        Assert.AreEqual(awaitable, CallsSourceGenerator.LastGeneration.Methods[0].Awaitable);
    }

    public static void AsssetDiagnostics(GenerationResult result)
    {
        Assert.IsTrue(result.RunResult.Diagnostics.IsEmpty);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        Assert.AreEqual(1, result.RunResult.GeneratedTrees.Length);
        var outputDiagnostics = result.Compilation.GetDiagnostics();
        Assert.IsFalse(outputDiagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
    }

    public static void AsssetNoGenerated(GenerationResult result)
    {
        Assert.IsTrue(result.RunResult.Diagnostics.IsEmpty);
        Assert.IsFalse(result.Diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error));
        Assert.AreEqual(0, result.RunResult.GeneratedTrees.Length);
    }
}
