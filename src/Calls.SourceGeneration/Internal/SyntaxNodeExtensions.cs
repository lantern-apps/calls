using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Calls.SourceGeneration;

internal static class SyntaxNodeExtensions
{
    //private static readonly SymbolDisplayFormat s_metadataDisplayFormat = SymbolDisplayFormat.QualifiedNameArityFormat.AddCompilerInternalOptions(SymbolDisplayCompilerInternalOptions.UsePlusForNestedTypes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPartial(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPublic(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNested(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Parent is TypeDeclarationSyntax;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAbstract(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStatic(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));

    public static bool IsDerivedFrom(this TypeDeclarationSyntax typeDeclaration, string baseTypeName, CancellationToken cancellationToken)
    {
        if (typeDeclaration.BaseList == null)
            return false;

        foreach (var baseType in typeDeclaration.BaseList.Types)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string baseName;
            if (baseType.Type is QualifiedNameSyntax qualifiedName)
            {
                baseName = qualifiedName.Right.Identifier.ValueText;
            }
            else if (baseType.Type is IdentifierNameSyntax identifierName)
            {
                baseName = identifierName.Identifier.ValueText;
            }
            else
            {
                Debug.Fail("Unhandle type");
                continue;
            }

            if (baseName == baseTypeName)
            {
                return true;
            }
        }

        return false;
    }
}
