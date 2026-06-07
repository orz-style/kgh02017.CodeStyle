using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Readability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferSwitchExpressionAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferSwitchExpression,
            "Prefer switch expression",
            "Use a switch expression instead of a switch statement",
            DiagnosticCategories.Readability,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeSwitchStatement, SyntaxKind.SwitchStatement);
    }

    private static void AnalyzeSwitchStatement(SyntaxNodeAnalysisContext context)
    {
        var switchStatement = (SwitchStatementSyntax)context.Node;

        // Only analyze switch statements with switch sections that contain a single return statement.
        if (switchStatement.Sections.Count == 0)
        {
            return;
        }

        foreach (SwitchSectionSyntax section in switchStatement.Sections)
        {
            if (!ContainsSingleReturnStatement(section))
            {
                return;
            }
        }

        var diagnostic = Diagnostic.Create(s_rule, switchStatement.SwitchKeyword.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }

    private static bool ContainsSingleReturnStatement(SwitchSectionSyntax section)
    {
        if (section.Statements.Count != 1)
        {
            return false;
        }

        return section.Statements[0] switch
        {
            ReturnStatementSyntax => true,
            BlockSyntax block =>
                    block.Statements.Count == 1 &&
                    block.Statements[0] is ReturnStatementSyntax,
            _ => false,
        };
    }
}
