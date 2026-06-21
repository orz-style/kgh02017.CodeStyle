using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferConsistentMultilineParametersAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferConsistentMultilineParameters,
            "Prefer consistent multiline parameters",
            "Use either a single-line parameter list or one parameter per line",
            DiagnosticCategories.Formatting,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var declaration = (MethodDeclarationSyntax)context.Node;
        ParameterListSyntax parameterList = declaration.ParameterList;
        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

        if(parameters.Count <= 1)
        {
            return;
        }

        int openParenLine = parameterList.OpenParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endParenLine = parameterList.CloseParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;

        if (openParenLine == endParenLine)
        {
            return;
        }

        for (int i = 1; i < parameters.Count; i++)
        {
            int previousLine = parameters[i - 1].GetLocation().GetLineSpan().StartLinePosition.Line;
            int currentLine = parameters[i].GetLocation().GetLineSpan().StartLinePosition.Line;

            if (previousLine == currentLine)
            {
                context.ReportDiagnostic(Diagnostic.Create(s_rule, parameterList.GetLocation()));
                return;
            }
        }
    }
}
