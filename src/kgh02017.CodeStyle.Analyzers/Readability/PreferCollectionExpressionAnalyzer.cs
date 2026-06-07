using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Readability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferCollectionExpressionAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferCollectionExpression,
            "Prefer collection expression",
            "Use a collection expression when the collection type is apparent",
            DiagnosticCategories.Readability,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeImplicitObjectCreation, SyntaxKind.ImplicitObjectCreationExpression);
    }

    private static void AnalyzeImplicitObjectCreation(SyntaxNodeAnalysisContext context)
    {
        var implicitObjectCreation = (ImplicitObjectCreationExpressionSyntax)context.Node;

        TypeInfo typeInfo = context.SemanticModel.GetTypeInfo(implicitObjectCreation, context.CancellationToken);

        if (typeInfo.Type is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        if (!IsSupportedCollectionType(typeSymbol))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, implicitObjectCreation.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsSupportedCollectionType(INamedTypeSymbol typeSymbol)
    {
        string typeName = typeSymbol.ConstructedFrom.ToDisplayString();

        return typeName is
            "System.Collections.Generic.List<T>" or
            "System.Collections.Generic.HashSet<T>" or
            "System.Collections.Generic.Dictionary<TKey, TValue>";
    }
}
