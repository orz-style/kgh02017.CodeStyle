using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Nullability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferIsNullAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferIsNull,
            "Prefer is null",
            "Use 'is null' or 'is not null' instead of equality operators",
            DiagnosticCategories.Nullability,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeBinaryExpression,
            SyntaxKind.EqualsExpression,
            SyntaxKind.NotEqualsExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        if (!IsNullLiteral(binaryExpression.Left) &&
            !IsNullLiteral(binaryExpression.Right))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, binaryExpression.OperatorToken.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsNullLiteral(ExpressionSyntax expression)
    {
        return expression.IsKind(SyntaxKind.NullLiteralExpression);
    }
}
