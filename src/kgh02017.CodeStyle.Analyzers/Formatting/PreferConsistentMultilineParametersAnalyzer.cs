using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Formatting;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class PreferConsistentMultilineParametersAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Prefer consistent multiline parameters";
    private const string Message = "Place all parameters on a single line or one parameter per line";
    private const string Description =
        "When a parameter list spans multiple lines, use a consistent layout by placing either "
        + "one parameter per line or all parameters on a single line.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferConsistentMultilineParameters,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Formatting,
            description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeParameterList, SyntaxKind.ParameterList);
    }

    private void AnalyzeParameterList(SyntaxNodeAnalysisContext context)
    {
        var parameterList = (ParameterListSyntax)context.Node;
        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

        if (parameters.Count <= 1)
        {
            return;
        }

        int openParenLine = parameterList.OpenParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int endParenLine = parameterList.CloseParenToken.GetLocation().GetLineSpan().StartLinePosition.Line;
        int firstParameterLine = parameters[0].GetLocation().GetLineSpan().StartLinePosition.Line;

        if (openParenLine == endParenLine)
        {
            return;
        }
        else if (openParenLine == firstParameterLine)
        {
            context.ReportDiagnostic(Diagnostic.Create(s_rule, parameterList.GetLocation()));
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
