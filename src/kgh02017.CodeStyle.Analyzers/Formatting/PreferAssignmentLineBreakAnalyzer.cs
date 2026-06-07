using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferAssignmentLineBreakAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferAssignmentLineBreak,
            "Prefer assignment line break",
            "Place a line break after the assignment operator",
            DiagnosticCategories.Formatting,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
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
}
