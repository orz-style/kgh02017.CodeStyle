using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Readability;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferNamedArgumentsForLiteralsAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Prefer named arguments for literals";
    private const string Message = "Use named arguments for null and boolean literal arguments";
    private const string Description =
        "When passing true, false, or null literals as arguments, use named arguments.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferNamedArgumentsForLiterals,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Readability,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeArgument, SyntaxKind.Argument);
    }

    private static void AnalyzeArgument(SyntaxNodeAnalysisContext context)
    {
        var argument = (ArgumentSyntax)context.Node;

        if (argument.NameColon is not null)
        {
            return;
        }

        if (!argument.Expression.IsKind(SyntaxKind.NullLiteralExpression)
            && !argument.Expression.IsKind(SyntaxKind.TrueLiteralExpression)
            && !argument.Expression.IsKind(SyntaxKind.FalseLiteralExpression))
        {
            return;
        }

        var diagnostic = Diagnostic.Create(s_rule, argument.GetLocation());
        context.ReportDiagnostic(diagnostic);
    }
}
