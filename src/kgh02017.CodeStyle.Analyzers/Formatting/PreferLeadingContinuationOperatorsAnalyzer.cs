using System.Collections.Immutable;
using System.Linq.Expressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferLeadingContinuationOperatorsAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferLeadingContinuationOperators,
            "Prefer leading continuation operators",
            "Place binary operators at the beginning of the continued line",
            DiagnosticCategories.Formatting,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeBinaryExpression,
            SyntaxKind.LogicalAndExpression,
            SyntaxKind.LogicalOrExpression,
            SyntaxKind.CoalesceExpression);
        context.RegisterSyntaxNodeAction(
            AnalyzeConditionalExpression,
            SyntaxKind.ConditionalExpression);
    }

    private static void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpression = (BinaryExpressionSyntax)context.Node;

        int leftLine = binaryExpression.Left.GetLocation().GetLineSpan().StartLinePosition.Line;
        int operatorTokenLine = binaryExpression.OperatorToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int expressionStartLine = binaryExpression.GetLocation().GetLineSpan().StartLinePosition.Line;
        int expressionEndLine = binaryExpression.GetLocation().GetLineSpan().EndLinePosition.Line;

        if (expressionStartLine == expressionEndLine)
        {
            return;
        }

        if (leftLine == operatorTokenLine)
        {
            context.ReportDiagnostic(Diagnostic.Create(s_rule, binaryExpression.OperatorToken.GetLocation()));
        }
    }

    private static void AnalyzeConditionalExpression(SyntaxNodeAnalysisContext context)
    {
        var conditional = (ConditionalExpressionSyntax)context.Node;

        int expressionStartLine = conditional.GetLocation().GetLineSpan().StartLinePosition.Line;
        int expressionEndLine = conditional.GetLocation().GetLineSpan().EndLinePosition.Line;
        int conditionLine = conditional.Condition.GetLocation().GetLineSpan().StartLinePosition.Line;
        int questionTokenLine = conditional.QuestionToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int whenTrueLine = conditional.WhenTrue.GetLocation().GetLineSpan().StartLinePosition.Line;
        int colonTokenLine = conditional.ColonToken.GetLocation().GetLineSpan().StartLinePosition.Line;

        if (expressionStartLine == expressionEndLine)
        {
            return;
        }

        if (conditionLine == questionTokenLine)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    s_rule,
                    conditional.QuestionToken.GetLocation()));
        }

        if (whenTrueLine == colonTokenLine)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    s_rule,
                    conditional.ColonToken.GetLocation()));
        }
    }

}