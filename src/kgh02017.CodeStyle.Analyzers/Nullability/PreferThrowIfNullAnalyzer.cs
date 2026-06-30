using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Nullability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferThrowIfNullAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Prefer ArgumentNullException.ThrowIfNull";
    private const string Message =
        "Use ArgumentNullException.ThrowIfNull instead of throwing ArgumentNullException manually";
    private const string Description =
        "When validating arguments, use ArgumentNullException."
        + "ThrowIfNull instead of throwing ArgumentNullException manually.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferThrowIfNull,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Nullability,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeIfStatement, SyntaxKind.IfStatement);
    }

    private static void AnalyzeIfStatement(SyntaxNodeAnalysisContext context)
    {
        var ifStatement = (IfStatementSyntax)context.Node;

        if (ifStatement.Condition is not IsPatternExpressionSyntax isPatternExpression)
        {
            return;
        }

        if (isPatternExpression.Pattern is not ConstantPatternSyntax constantPattern ||
            !constantPattern.Expression.IsKind(SyntaxKind.NullLiteralExpression))
        {
            return;
        }

        if (ifStatement.Statement is not BlockSyntax block ||
            block.Statements.Count != 1)
        {
            return;
        }

        if (block.Statements[0] is not ThrowStatementSyntax throwStatement)
        {
            return;
        }

        if (throwStatement.Expression is not ObjectCreationExpressionSyntax objectCreation)
        {
            return;
        }

        if (objectCreation.Type.ToString() != "ArgumentNullException")
        {
            return;
        }

        if (objectCreation.ArgumentList?.Arguments.Count != 1)
        {
            return;
        }

        ArgumentSyntax argument = objectCreation.ArgumentList.Arguments[0];

        if (argument.Expression is not InvocationExpressionSyntax nameofInvocation)
        {
            return;
        }

        if (nameofInvocation.Expression is not IdentifierNameSyntax identifierName ||
            identifierName.Identifier.ValueText != "nameof")
        {
            return;
        }

        if (nameofInvocation.ArgumentList.Arguments.Count != 1)
        {
            return;
        }

        if (nameofInvocation.ArgumentList.Arguments[0].Expression is not IdentifierNameSyntax nameofTarget)
        {
            return;
        }

        if (isPatternExpression.Expression is not IdentifierNameSyntax checkedTarget)
        {
            return;
        }

        if (nameofTarget.Identifier.ValueText != checkedTarget.Identifier.ValueText)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, ifStatement.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }

}
