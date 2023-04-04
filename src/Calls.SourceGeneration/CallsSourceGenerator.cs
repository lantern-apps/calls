using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;

namespace Calls.SourceGeneration;

[Generator(LanguageNames.CSharp)]
public partial class CallsSourceGenerator : IIncrementalGenerator
{
    const string ObjectTypeCompilableName = "global::System.Object";
    const string CoreNamespace = "Calls";
    const string CallableAttributeName = CoreNamespace + ".CallableAttribute";

    const string MediatorBaseTypeSimpleName = "Call";
    const string MediatorBaseTypeName = CoreNamespace + ".Call";

    const string IncludeTypeAttributeName = CoreNamespace + ".IncludeTypeAttribute";
    const string IncludeAssemblyAttributeName = CoreNamespace + ".IncludeAssemblyAttribute";

    const string TaskTypeName = "System.Threading.Tasks.Task";
    const string TaskTypeCompilableName = "global::System.Threading.Tasks.Task";

    //private static readonly DiagnosticDescriptor MissPartialKeywordWarning = new DiagnosticDescriptor(id: "CALLGEN001",
    //                                                                                          title: "Miss partial keyword",
    //                                                                                          messageFormat: "Couldn't parse XML file '{0}'.",
    //                                                                                          category: "MediatorSourceGenerator",
    //                                                                                          DiagnosticSeverity.Error,
    //                                                                                          isEnabledByDefault: true);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var callerDeclarations = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, token) =>
            {
                if (node is not ClassDeclarationSyntax classDeclaration
                    || !classDeclaration.IsPartial()
                    || classDeclaration.IsNested()
                    || classDeclaration.IsAbstract()
                    || classDeclaration.IsStatic()
                    || classDeclaration.TypeParameterList != null)
                    return false;

                if (classDeclaration.BaseList == null || classDeclaration.BaseList.Types.Count == 0)
                    return false;

                return classDeclaration.IsDerivedFrom(MediatorBaseTypeSimpleName, token);
            },
            transform: static (context, token) =>
            {
                return (ClassDeclarationSyntax)context.Node;
            });

        var methodDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            CallableAttributeName,
            predicate: static (node, token) =>
            {
                if (node is not MethodDeclarationSyntax method
                    || method.IsPartial()
                    || method.IsStatic()
                    || method.TypeParameterList != null)
                    return false;

                if (node.Parent is ClassDeclarationSyntax classDeclaration)
                {
                    return !classDeclaration.IsStatic() &&
                        classDeclaration.TypeParameterList == null &&
                        method.IsPublic();
                }
                else if (node.Parent is InterfaceDeclarationSyntax interfaceDeclaration)
                {
                    return interfaceDeclaration.TypeParameterList == null;
                }

                return false;
            },
            transform: static (context, token) =>
            {
                return (MethodDeclarationSyntax)context.TargetNode;
            });

        var source = callerDeclarations
            .Combine(methodDeclarations.Collect())
            .Combine(context.CompilationProvider);

        context.RegisterSourceOutput(source, static (sourceContext, source) =>
        {
            CancellationToken cancellationToken = sourceContext.CancellationToken;

            var callerDeclaration = source.Left.Left;
            var methodDeclarations = source.Left.Right;
            var compilation = source.Right;

            Parser parser = new(callerDeclaration, methodDeclarations, compilation, cancellationToken);
            var meta = parser.Parse();

            if (meta == null)
                return;

            var script = Emit(meta, cancellationToken);

            string output;
            if (meta.Namespace != null)
            {
                output = $"{meta.Namespace}.{meta.Name}.g.cs";
            }
            else
            {
                output = $"{meta.Name}.g.cs";
            }

            sourceContext.AddSource(output, script);

#if Test
            LastGeneration = meta;
#endif
        });
    }

#if Test

    public static Calls? LastGeneration { get; private set; }

#endif

#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。

#if Test
    public
#else
    private
#endif
        sealed class Calls
    {
        public string Name;
        public string? Namespace;
        public MethodDescription[] Methods;
    }

#if Test
    public
#else
    private
#endif
        sealed class MethodDescription
    {
        public string? RoutingKey;
        public string Type;
        public string MethodName;
        public string? ReturnType;
        public ParameterDescription[] Parameters;
        public bool Awaitable;
    }

    public sealed class ParameterDescription
    {
        public string Type;
        public bool HasDefaultValue;
        public string DefaultValue;
        public bool Nullable;
    }
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
}
