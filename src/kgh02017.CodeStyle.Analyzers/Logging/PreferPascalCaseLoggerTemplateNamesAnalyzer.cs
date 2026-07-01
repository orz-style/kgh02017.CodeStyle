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
    private const string Title = "Prefer PascalCase logger template names";
    private const string Message = "Use PascalCase for structured logging placeholder names";
    private const string Description =
        "When using structured logging placeholders, use the PascalCase naming convention.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.PreferPascalCaseLoggerTemplateNames,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Logging,
            description: Description);

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

        if (!methodName.StartsWith("Log", StringComparison.Ordinal))
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
