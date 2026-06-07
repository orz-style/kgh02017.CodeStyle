using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Logging;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferPascalCaseLoggerTemplateNamesAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.PreferPascalCaseLoggerTemplateNames,
            "Prefer PascalCase logger template names",
            "Use PascalCase for structured logging placeholder names",
            DiagnosticCategories.Logging,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    private static readonly Regex s_templateRegex = new(@"\{(?<name>[^\}]+)\}", RegexOptions.Compiled);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;

        if (invocation.Expression is not MemberAccessExpressionSyntax memberAccess)
        {
            return;
        }

        string methodName = memberAccess.Name.Identifier.ValueText;

        if (!methodName.StartsWith("Log"))
        {
            return;
        }

        if (invocation.ArgumentList.Arguments.Count == 0)
        {
            return;
        }

        ArgumentSyntax firstArgument = invocation.ArgumentList.Arguments[0];

        if (firstArgument.Expression is not LiteralExpressionSyntax literalExpression)
        {
            return;
        }

        if (!literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
        {
            return;
        }

        string template = literalExpression.Token.ValueText;

        foreach (Match match in s_templateRegex.Matches(template))
        {
            string name = match.Groups["name"].Value;

            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            if (!char.IsUpper(name[0]))
            {
                var diagnostic = Diagnostic.Create(s_rule, firstArgument.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }

    }
}
