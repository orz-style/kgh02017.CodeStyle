using System.Collections.Immutable;
using System.Composition;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Readability;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferNamedArgumentsForLiteralsCodeFixProvider))]
[Shared]
public sealed class PreferNamedArgumentsForLiteralsCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferNamedArgumentsForLiterals];

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        SyntaxNode? root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        if (root is null)
        {
            return;
        }

        Diagnostic diagnostic = context.Diagnostics[0];

        TextSpan span = diagnostic.Location.SourceSpan;

        ArgumentSyntax? argument =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<ArgumentSyntax>()
                .FirstOrDefault();

        if (argument is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use named arguments",
                cancellationToken =>
                    UseNamedArgumentAsync(context.Document, argument, cancellationToken),
                nameof(PreferNamedArgumentsForLiteralsCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> UseNamedArgumentAsync(
        Document document,
        ArgumentSyntax argument,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        SemanticModel? semanticModel = await document.GetSemanticModelAsync(cancellationToken);

        if (semanticModel is null)
        {
            return document;
        }

        InvocationExpressionSyntax? invocation =
            argument.Ancestors()
                .OfType<InvocationExpressionSyntax>()
                .FirstOrDefault();

        if (invocation is null)
        {
            return document;
        }

        if (semanticModel.GetSymbolInfo(
                invocation,
                cancellationToken: cancellationToken).Symbol is not IMethodSymbol method)
        {
            return document;
        }

        int index = invocation.ArgumentList.Arguments.IndexOf(argument);

        if (index < 0 || index >= method.Parameters.Length)
        {
            return document;
        }

        string parameterName = method.Parameters[index].Name;

        ArgumentSyntax replacement =
            argument
                .WithNameColon(
                    SyntaxFactory.NameColon(
                        SyntaxFactory.IdentifierName(parameterName))
                    .WithTrailingTrivia(SyntaxFactory.Space))
                .WithTriviaFrom(argument);

        SyntaxNode newRoot = root.ReplaceNode(argument, replacement);

        return document.WithSyntaxRoot(newRoot);
    }
}
