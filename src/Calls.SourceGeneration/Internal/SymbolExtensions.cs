using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace Calls.SourceGeneration;

internal static class SymbolExtensions
{
    public static bool IsEnum(this ITypeSymbol symobl) => symobl.BaseType != null && symobl.BaseType.ToDisplayString() == "System.Enum";

    public static bool IsNullable(this ITypeSymbol symobl) => symobl is INamedTypeSymbol namedTypeSymbol
               && namedTypeSymbol.IsGenericType
               && namedTypeSymbol.ConstructedFrom.ToDisplayString() == "System.Nullable<T>";

    public static string? GetNamespaceDisplayString(this ITypeSymbol symbol)
    {
        if (symbol is IArrayTypeSymbol arrayTypeSymbol)
        {
            return arrayTypeSymbol.ElementType.GetNamespaceDisplayString();
        }
        else
        {
            return symbol.ContainingNamespace.IsGlobalNamespace ? null :
                symbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat
                        .WithGlobalNamespaceStyle(SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining));
        }
    }

    public static AttributeData? GetAttribute(ISymbol symbol, string attributeTypeName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == attributeTypeName)
                return attribute;
        }

        return null;
    }

    public static IEnumerable<AttributeData> GetAttributes(ISymbol symbol, string attributeTypeName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == attributeTypeName)
                yield return attribute;
        }
    }

    public static bool HasAttribute(this ISymbol symbol, string attributeTypeName)
    {
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() == attributeTypeName)
                return true;
        }
        return false;
    }

    public static IEnumerable<ITypeSymbol> EnumerableTypes(this IAssemblySymbol assembly)
    {
        Queue<INamespaceSymbol> queue = new(new INamespaceSymbol[] { assembly.GlobalNamespace });
        while (queue.Count > 0)
        {
            var ns = queue.Dequeue();
            foreach (var nsOrType in ns.GetMembers())
            {
                if (nsOrType is ITypeSymbol typeSymbol)
                    yield return typeSymbol;
                else
                    queue.Enqueue((INamespaceSymbol)nsOrType);
            }
        }
    }
}