using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace Calls.SourceGeneration;

internal static class SyntaxNodeExtensions
{
    public static bool IsPartial(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    
    public static bool IsPublic(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword));

    public static bool IsNested(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Parent is TypeDeclarationSyntax;

    public static bool IsAbstract(this MemberDeclarationSyntax typeDeclaration) => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));

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
