using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace kgh02017.CodeStyle.Analyzers.Logging;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DoNotUseInterpolatedStringInLoggerAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor s_rule =
        new(
            DiagnosticIds.DoNotUseInterpolatedStringInLogger,
            "Do not use interpolated string in logger calls",
            "Do not use interpolated string in logger calls. Use structured logging instead.",
            DiagnosticCategories.Logging,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

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
