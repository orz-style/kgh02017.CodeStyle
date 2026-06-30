using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferAssignmentLineBreakAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Prefer assignment line break";
    private const string Message = "Place a line break after the assignment operator";
    private const string Description =
        "When the right-hand side of an assignment spans multiple lines, "
        + "place the assigned value on the following line.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferAssignmentLineBreak,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Formatting,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(
            AnalyzeAssignmentExpression,
            SyntaxKind.SimpleAssignmentExpression,
            SyntaxKind.AddAssignmentExpression);
    }

    private static void AnalyzeDeclarationStatement(SyntaxNodeAnalysisContext context)
    {
        var declaration = (LocalDeclarationStatementSyntax)context.Node;

        if (declaration.Declaration.Variables.Count != 1)
        {
            return;
        }

        VariableDeclaratorSyntax variable = declaration.Declaration.Variables[0];

        if (variable.Initializer is null)
        {
            return;
        }

        EqualsValueClauseSyntax initializer = variable.Initializer;
        ExpressionSyntax value = initializer.Value;

        FileLinePositionSpan valueSpan = value.GetLocation().GetLineSpan();
        FileLinePositionSpan equalsSpan = initializer.EqualsToken.GetLocation().GetLineSpan();

        bool isMultiLine = valueSpan.StartLinePosition.Line != valueSpan.EndLinePosition.Line;
        bool startsOnEqualsLine = valueSpan.StartLinePosition.Line == equalsSpan.StartLinePosition.Line;

        if (!isMultiLine || !startsOnEqualsLine)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(s_rule, initializer.EqualsToken.GetLocation()));
    }

    private static void AnalyzeAssignmentExpression(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;

        ExpressionSyntax value = assignment.Right;
        SyntaxToken operatorToken = assignment.OperatorToken;

        FileLinePositionSpan valueSpan = value.GetLocation().GetLineSpan();
        FileLinePositionSpan operatorTokenSpan = operatorToken.GetLocation().GetLineSpan();

        bool isMultiLine = valueSpan.StartLinePosition.Line != valueSpan.EndLinePosition.Line;
        bool startsOnOperatorLine = valueSpan.StartLinePosition.Line == operatorTokenSpan.StartLinePosition.Line;

        if (!isMultiLine || !startsOnOperatorLine)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(s_rule, operatorToken.GetLocation()));
    }
}
