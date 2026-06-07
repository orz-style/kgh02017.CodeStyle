using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MaximumLineLengthAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.MaximumLineLength,
            "Line exceeds 120 characters",
            "Wrap the text so that line length is less than 120 characters",
            DiagnosticCategories.Formatting,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeSyntaxTree);
    }

    private static void AnalyzeSyntaxTree(SyntaxTreeAnalysisContext context)
    {
        SourceText text = context.Tree.GetText(context.CancellationToken);

        foreach (TextLine line in text.Lines)
        {
            if (line.Span.Length <= 120)
            {
                continue;
            }

            var location = Location.Create(context.Tree, new TextSpan(line.Start, 1));

            var diagnostic = Diagnostic.Create(s_rule, location);

            context.ReportDiagnostic(diagnostic);
        }
    }
}
