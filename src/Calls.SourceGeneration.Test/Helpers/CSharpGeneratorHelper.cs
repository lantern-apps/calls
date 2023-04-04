using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Reflection;

namespace Calls.SourceGeneration.Test;

public static class CSharpGeneratorHelper
{
    public static GenerationResult Generate(string source, params Assembly[] addReferences)
    {
        var generator = new CallsSourceGenerator();
        var result = CSharpGeneratorDriver
            .Create(generator)
            .RunGeneratorsAndUpdateCompilation(
                CreateLibrary(source, addReferences),
                out var outputCompilation,
                out var diagnostics)
            .GetRunResult();

        return new(result, outputCompilation, diagnostics);
    }

    private static readonly Assembly[] ImportantAssemblies = new[]
    {
        typeof(object).Assembly,
        typeof(Call).Assembly,
        typeof(CallsSourceGenerator).Assembly,
        typeof(MulticastDelegate).Assembly
    };

    private static Assembly[] AssemblyReferencesForCodegen =>
        AppDomain.CurrentDomain
            .GetAssemblies()
            .Concat(ImportantAssemblies)
            .Distinct()
            .Where(a => !a.IsDynamic)
            .ToArray();

    private static Compilation CreateLibrary(string source, params Assembly[] addReferences)
    {
        var references = new List<MetadataReference>();
        var assemblies = AssemblyReferencesForCodegen;
        foreach (Assembly assembly in assemblies)
        {
            if (!assembly.IsDynamic)
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }
        if (addReferences != null)
        {
            foreach (var assembly in addReferences)
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        var compilation = CSharpCompilation.Create(
            "compilation",
            new SyntaxTree[] { CSharpSyntaxTree.ParseText(source) },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        return compilation;
    }

    private static Task<string> SourceFromResourceFile(string file) => File.ReadAllTextAsync(Path.Combine("resources", file));

}

public class GenerationResult
{
    public GenerationResult(
        GeneratorDriverRunResult runResult,
        Compilation compilation,
        ImmutableArray<Diagnostic> diagnostics)
    {
        RunResult = runResult;
        Compilation = compilation;
        Diagnostics = diagnostics;
    }

    public GeneratorDriverRunResult RunResult { get; }
    public Compilation Compilation { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }
}
