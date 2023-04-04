using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Calls.SourceGeneration;

public partial class CallsSourceGenerator
{
    private sealed class Parser
    {
        static SymbolDisplayFormat DisplayFormat = new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Included, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces)
            .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
            .AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.UseSpecialTypes/* | SymbolDisplayMiscellaneousOptions.ExpandNullable*/);

        private readonly ClassDeclarationSyntax _callerDeclaration;
        private readonly INamedTypeSymbol _callerTypeSymbol;
        private readonly ImmutableArray<MethodDeclarationSyntax> _methodDeclarations;
        private readonly Compilation _compilation;
        private readonly CancellationToken _cancellation;
        private readonly List<INamedTypeSymbol> _callableTypes = new(10);
        private readonly List<MethodDescription> _methods = new(40);

        public Parser(
            ClassDeclarationSyntax caller,
            ImmutableArray<MethodDeclarationSyntax> methods,
            Compilation compilation,
            CancellationToken cancellationToken)
        {
            _callerDeclaration = caller;
            _methodDeclarations = methods;
            _compilation = compilation;
            _cancellation = cancellationToken;

            var semanticModel = _compilation.GetSemanticModel(_callerDeclaration.SyntaxTree);
            Debug.Assert(semanticModel != null);

            _callerTypeSymbol = semanticModel.GetDeclaredSymbol(_callerDeclaration, _cancellation)!;
            Debug.Assert(_callerTypeSymbol != null);
            Debug.Assert(_callerTypeSymbol!.BaseType != null);
            Debug.Assert(_callerTypeSymbol!.BaseType!.ToDisplayString() == MediatorBaseTypeName);
        }

        public Calls? Parse()
        {
            LoadCallableTypes();

            if (_includeCurrent)
            {
                LoadCurrentAssemblyMethods();
            }

            if (_methods.Count == 0)
            {
                return null;
            }

            return new Calls
            {
                Name = _callerTypeSymbol.Name, //callerTypeSymbol.ToDisplayString(new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypes)),
                Namespace = _callerTypeSymbol.GetNamespaceDisplayString(),
                Methods = _methods.ToArray(),
            };
        }

        private void LoadCurrentAssemblyMethods()
        {
            foreach (var group in _methodDeclarations.GroupBy(c => c.SyntaxTree))
            {
                SyntaxTree syntaxTree = group.Key;
                SemanticModel compilationSemanticModel = _compilation.GetSemanticModel(syntaxTree);

                foreach (var methodGroup in group.GroupBy(x => x.Parent))
                {
                    if (compilationSemanticModel.GetDeclaredSymbol(methodGroup.Key!, _cancellation) is not INamedTypeSymbol callableType)
                        continue;

                    if (_callableTypes.Contains(callableType))
                        continue;

                    string typeName = callableType.ToDisplayString(DisplayFormat);

                    foreach (var method in methodGroup)
                    {
                        _cancellation.ThrowIfCancellationRequested();

                        var methodSymbol = compilationSemanticModel.GetDeclaredSymbol(method, _cancellation);

                        if (methodSymbol != null)
                        {
                            ParseMethod(typeName, methodSymbol);
                        }
                    }
                }
            }
        }

        private bool _includeCurrent = false;

        private void LoadCallableTypes()
        {
            List<IAssemblySymbol> callableAssemblies = new(4);

            foreach (var attribute in _callerTypeSymbol.GetAttributes())
            {
                _cancellation.ThrowIfCancellationRequested();

                var attributeName = attribute.AttributeClass?.ToDisplayString();

                if (attributeName == IncludeTypeAttributeName)
                {
                    Debug.Assert(attribute.ConstructorArguments.Length == 1);
                    Debug.Assert(attribute.ConstructorArguments[0].Value is INamedTypeSymbol);

                    ParseCallableType((INamedTypeSymbol)attribute.ConstructorArguments[0].Value!);
                }
                else if (attributeName == IncludeAssemblyAttributeName)
                {
                    var namedTypeSymbol = (INamedTypeSymbol?)attribute.ConstructorArguments[0].Value;
                    if (namedTypeSymbol == null)
                    {
                        _includeCurrent = true;
                        continue;
                    }

                    var assembly = namedTypeSymbol.ContainingAssembly;
                    if(SymbolEqualityComparer.Default.Equals(_compilation.Assembly, assembly))
                    {
                        _includeCurrent = true;
                        continue;
                    }

                    if (callableAssemblies.Contains(assembly))
                    {
                        continue;
                    }

                    callableAssemblies.Add(assembly);

                    foreach (var callableType in assembly.EnumerableTypes()
                        .OfType<INamedTypeSymbol>()
                        .Where(x => x.DeclaredAccessibility == Accessibility.Public
                                && !x.IsStatic
                                && !x.IsValueType
                                && !x.IsUnmanagedType
                                && !x.IsTupleType
                                && !x.IsGenericType))
                    {
                        _cancellation.ThrowIfCancellationRequested();

                        ParseCallableType(callableType);
                    }
                }
            }
        }

        private void ParseCallableType(INamedTypeSymbol typeSymbol)
        {
            if (_callableTypes.Contains(typeSymbol))
                return;
            _callableTypes.Add(typeSymbol);

            string? typeName = null;

            foreach (var method in typeSymbol.GetMembers()
                .OfType<IMethodSymbol>()
                .Where(x => x.DeclaredAccessibility == Accessibility.Public && x.IsGenericMethod == false))
            {
                _cancellation.ThrowIfCancellationRequested();

                var routingKeys = GetRoutingKeys(method);
                if (routingKeys.Count == 0)
                    continue;

                ParseMethod(typeName ??= typeSymbol.ToDisplayString(DisplayFormat), method, routingKeys);
            }
        }

        private bool TryParseParameter(IParameterSymbol parameter, out ParameterDescription? parameterDescription)
        {
            parameterDescription = null;
            if (parameter.RefKind != RefKind.None && parameter.RefKind != RefKind.In)
                return false;

            var typeSymbol = parameter.Type;

            if (typeSymbol.Kind == SymbolKind.TypeParameter)
                return false;

            string? type = typeSymbol.ToDisplayString(DisplayFormat);
            string? defaultValue = null;
            if (parameter.HasExplicitDefaultValue)
            {
                if (typeSymbol.IsEnum())
                {
                    foreach (var field in typeSymbol.GetMembers().OfType<IFieldSymbol>())
                    {
                        if (parameter.ExplicitDefaultValue?.Equals(field.ConstantValue) == true)
                        {
                            defaultValue = type + '.' + field.Name;
                            break;
                        }
                    }
                }
                else
                {
                    defaultValue = ToScriptString(parameter.ExplicitDefaultValue);
                }
            }

            var nullable = !typeSymbol.IsValueType || typeSymbol.IsNullable();

            parameterDescription = new ParameterDescription
            {
                Type = type,
                HasDefaultValue = parameter.HasExplicitDefaultValue,
                DefaultValue = defaultValue ?? "null",
                Nullable = nullable,
            };
            return true;
        }

        private void ParseMethod(string typeName, IMethodSymbol methodSymbol, IReadOnlyList<string?>? routingKeys = null)
        {
            int parameterCount = methodSymbol.Parameters.Length;
            routingKeys ??= GetRoutingKeys(methodSymbol);

            var parameters = new ParameterDescription[methodSymbol.Parameters.Length];
            for (int i = 0; i < parameterCount; i++)
            {
                if (!TryParseParameter(methodSymbol.Parameters[i], out var parameter))
                    return;
                parameters[i] = parameter;
            }

            ParseReturnType(methodSymbol, out string? returnType, out bool awaitable);

            foreach (var routeKey in routingKeys)
            {
                _methods.Add(new MethodDescription
                {
                    Type = typeName,
                    MethodName = methodSymbol.Name,
                    Parameters = parameters,
                    RoutingKey = routeKey,
                    ReturnType = returnType,
                    Awaitable = awaitable,
                });
            }
        }

        private static void ParseReturnType(IMethodSymbol methodSymbol, out string? returnType, out bool awaitable)
        {
            returnType = null;
            awaitable = false;
            if (methodSymbol.ReturnsVoid)
                return;

            if (methodSymbol.ReturnType is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.IsGenericType
                && namedTypeSymbol.BaseType?.ToDisplayString() == TaskTypeName)
            {
                returnType = namedTypeSymbol.TypeArguments[0].ToDisplayString(DisplayFormat);
                awaitable = true;
            }
            else
            {
                returnType = methodSymbol.ReturnType.ToDisplayString(DisplayFormat);
                if (returnType == TaskTypeCompilableName)
                {
                    returnType = null;
                    awaitable = true;
                }
                else
                {
                    awaitable = false;
                }
            }
        }

        private IReadOnlyList<string?> GetRoutingKeys(IMethodSymbol methodSymbol)
        {
            List<string?> routintKeys = new();
            foreach (var attribute in methodSymbol.GetAttributes())
            {
                _cancellation.ThrowIfCancellationRequested();

                if (attribute.AttributeClass?.ToDisplayString() != CallableAttributeName)
                    continue;

                string? routingKey;
                if (attribute.ConstructorArguments.Length == 0)
                {
                    routingKey = null;
                }
                else
                {
                    var value = attribute.ConstructorArguments[0].Value;
                    if (value is null)
                    {
                        routingKey = null;
                    }
                    else if (value is string stringValue)
                    {
                        routingKey = stringValue;
                    }
                    else if (value is bool boolValue)
                    {
                        routingKey = boolValue ? methodSymbol.Name : null;
                    }
                    else
                    {
                        continue;
                    }
                }

                if (!routintKeys.Contains(routingKey))
                {
                    routintKeys.Add(routingKey);
                }
            }

            return routintKeys;
        }
    }
}
