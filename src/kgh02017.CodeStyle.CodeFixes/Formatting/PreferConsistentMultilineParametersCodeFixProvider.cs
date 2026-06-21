using System.Collections.Immutable;
using System.Composition;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Formatting;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferConsistentMultilineParametersCodeFixProvider))]
[Shared]
public sealed class PreferConsistentMultilineParametersCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferConsistentMultilineParameters];

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

        ParameterListSyntax? parameterList =
            root.FindNode(span)
                .AncestorsAndSelf()
                .OfType<ParameterListSyntax>()
                .FirstOrDefault();

        if (parameterList is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use one parameter per line",
                cancellationToken =>
                    UseOneParameterPerLineAsync(context.Document, parameterList, cancellationToken),
                "UseOneParameterPerLine"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use single-line parameter list",
                cancellationToken =>
                    UseSingleLineParameterListAsync(context.Document, parameterList, cancellationToken),
                "UseSingleLineParameterList"),
            diagnostic);
    }

    private async Task<Document> UseOneParameterPerLineAsync(
        Document document,
        ParameterListSyntax parameterList,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (parameterList.Parameters.Count <= 1)
        {
            return document;
        }

        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);

        MethodDeclarationSyntax? declaration =
            parameterList.Ancestors()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault();

        string parameterIndent = GetIndentation(declaration?.GetLeadingTrivia() ?? default) + "    ";

        string text =
            "(" + newLine +
            parameterIndent +
            string.Join(
                "," + newLine + parameterIndent,
                parameters.Select(a => a.ToString())) +
            ")";

        ParameterListSyntax newParameterList = SyntaxFactory.ParseParameterList(text);

        newParameterList =
            newParameterList.WithCloseParenToken(
                newParameterList.CloseParenToken
                    .WithTrailingTrivia(parameterList.CloseParenToken.TrailingTrivia));

        SyntaxNode newRoot = root.ReplaceNode(parameterList, newParameterList);

        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> UseSingleLineParameterListAsync(
        Document document,
        ParameterListSyntax parameterList,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (parameterList.Parameters.Count <= 1)
        {
            return document;
        }

        SeparatedSyntaxList<ParameterSyntax> parameters = parameterList.Parameters;

        string text =
            "(" +
            string.Join(", ", parameters.Select(a => a.ToString())) +
            ")";

        ParameterListSyntax newParameterList = SyntaxFactory.ParseParameterList(text);

        newParameterList =
            newParameterList.WithCloseParenToken(
                newParameterList.CloseParenToken
                    .WithTrailingTrivia(parameterList.CloseParenToken.TrailingTrivia));

        SyntaxNode newRoot = root.ReplaceNode(parameterList, newParameterList);

        return document.WithSyntaxRoot(newRoot);
    }

    private static string GetIndentation(SyntaxTriviaList leadingTrivia)
    {
        return string.Concat(
            leadingTrivia
                .Reverse()
                .TakeWhile(trivia => trivia.IsKind(SyntaxKind.WhitespaceTrivia))
                .Reverse()
                .Select(trivia => trivia.ToFullString()));
    }
}
