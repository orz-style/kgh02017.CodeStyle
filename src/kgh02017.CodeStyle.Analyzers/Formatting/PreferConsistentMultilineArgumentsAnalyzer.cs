using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferConsistentMultilineArgumentsAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferConsistentMultilineArguments,
            "Prefer consistent multiline arguments",
            "Use either a single-line argument list or one argument per line",
            DiagnosticCategories.Formatting,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeArgumentList, SyntaxKind.ArgumentList);
    }

    private void AnalyzeArgumentList(SyntaxNodeAnalysisContext context)
    {
        var argumentList = (ArgumentListSyntax)context.Node;
        SeparatedSyntaxList<ArgumentSyntax> args = argumentList.Arguments;

        if (args.Count == 0)
        {
            return;
        }

        int openParenLine = argumentList.OpenParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endParenLine = argumentList.CloseParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int firstArgumentLine = args[0].GetLocation().GetLineSpan().StartLinePosition.Line;

        if (openParenLine == endParenLine)
        {
            return;
        }
        else if (openParenLine == firstArgumentLine)
        {
            context.ReportDiagnostic(Diagnostic.Create(s_rule, argumentList.GetLocation()));
            return;
        }

        for (int i = 1; i < args.Count; i++)
        {
            int previousLine = args[i - 1].GetLocation().GetLineSpan().StartLinePosition.Line;
            int currentLine = args[i].GetLocation().GetLineSpan().StartLinePosition.Line;

            if (previousLine == currentLine)
            {
                context.ReportDiagnostic(Diagnostic.Create(s_rule, argumentList.GetLocation()));
                return;
            }
        }
    }
}
