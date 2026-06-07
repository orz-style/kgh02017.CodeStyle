using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Strings;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SpecifyStringComparisonAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.SpecifyStringComparison,
            "Specify StringComparison",
            "Specify StringComparison explicitly",
            DiagnosticCategories.Strings,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        if (methodSymbol.ContainingType.SpecialType != SpecialType.System_String)
        {
            return;
        }

        string methodName = memberAccess.Name.Identifier.ValueText;

        if (methodName is not ("StartsWith" or "EndsWith" or "Contains"))
        {
            return;
        }

        if (invocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, memberAccess.Name.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }
}
