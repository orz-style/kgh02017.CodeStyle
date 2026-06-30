using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Strings;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SpecifyStringComparisonAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Specify StringComparison";
    private const string Message = "Specify StringComparison explicitly";
    private const string Description =
        "When calling string comparison APIs, specify the StringComparison argument explicitly.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.SpecifyStringComparison,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Strings,
            description: Description);

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
