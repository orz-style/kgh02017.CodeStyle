using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Logging;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DoNotUseInterpolatedStringInLoggerAnalyzer : DiagnosticAnalyzer
{
    private const string Title = "Do not use interpolated strings in logger calls";
    private const string Message = "Do not use interpolated strings in logger calls. Use structured logging instead.";
    private const string Description =
        "When using logging APIs, use structured logging instead of interpolated strings.";

    private static readonly DiagnosticDescriptor s_rule =
        DiagnosticDescriptorBuilder.CreateRule(
            id: DiagnosticIds.DoNotUseInterpolatedStringInLogger,
            title: Title,
            message: Message,
            category: DiagnosticCategories.Logging,
            description: Description);

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

        foreach (ArgumentSyntax argument in invocation.ArgumentList.Arguments)
        {
            if (argument.Expression is InterpolatedStringExpressionSyntax)
            {
                var diagnostic = Diagnostic.Create(s_rule, argument.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
