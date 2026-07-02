using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Readability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferUsingDeclarationAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Prefer using declaration";
    private const string Message = "Use a using declaration instead of a using statement";
    private const string Description = "When using a using statement, prefer a using declaration instead.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferUsingDeclaration,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Readability,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeUsingStatement, SyntaxKind.UsingStatement);
    }

    private static void AnalyzeUsingStatement(SyntaxNodeAnalysisContext context)
    {
        var usingStatement = (UsingStatementSyntax)context.Node;

        if (usingStatement.Declaration is null)
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, usingStatement.UsingKeyword.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }
}
