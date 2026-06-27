using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Formatting;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferConsistentMultilineArgumentsCodeFixProvider))]
[Shared]
public sealed class PreferConsistentMultilineArgumentsCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferConsistentMultilineArguments];

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

        ArgumentListSyntax? argumentList =
            root.FindNode(span)
                .AncestorsAndSelf()
                .OfType<ArgumentListSyntax>()
                .FirstOrDefault();

        if (argumentList is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use one argument per line",
                cancellationToken =>
                    UseOneArgumentPerLineAsync(context.Document, argumentList, cancellationToken),
                "UseOneArgumentPerLine"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use single-line argument list",
                cancellationToken =>
                    UseSingleLineArgumentListAsync(context.Document, argumentList, cancellationToken),
                "UseSingleLineArgumentList"),
            diagnostic);

    }

    private static async Task<Document> UseOneArgumentPerLineAsync(
        Document document,
        ArgumentListSyntax argumentList,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (argumentList.Arguments.Count == 0)
        {
            return document;
        }

        SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);

        StatementSyntax? statement =
            argumentList.Ancestors()
                .OfType<StatementSyntax>()
                .FirstOrDefault();

        SourceText sourceText = await document.GetTextAsync(cancellationToken);

        string baseIndent =
            GetLineIndentation(
                sourceText,
                argumentList.OpenParenToken.SpanStart);

        string argumentIndent = baseIndent + "    ";

        SyntaxTriviaList argumentLeadingTrivia =
            SyntaxFactory.TriviaList(
                SyntaxFactory.EndOfLine(newLine),
                SyntaxFactory.Whitespace(argumentIndent));


        SeparatedSyntaxList<ArgumentSyntax> newArguments =
            SyntaxFactory.SeparatedList(
                arguments.Select(argument =>
                    NormalizeArgumentForMultiLine(argument, newLine, argumentIndent)),
                arguments
                    .GetSeparators()
                    .Select(separator =>
                        separator.WithTrailingTrivia(argumentLeadingTrivia)));

        ArgumentListSyntax newArgumentList =
            argumentList
                .WithOpenParenToken(
                    argumentList.OpenParenToken.WithTrailingTrivia(argumentLeadingTrivia))
                .WithArguments(newArguments)
                .WithCloseParenToken(
                    argumentList.CloseParenToken
                        .WithLeadingTrivia()
                        .WithTrailingTrivia(argumentList.CloseParenToken.TrailingTrivia));

        SyntaxNode newRoot = root.ReplaceNode(argumentList, newArgumentList);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> UseSingleLineArgumentListAsync(
        Document document,
        ArgumentListSyntax argumentList,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (argumentList.Arguments.Count == 0)
        {
            return document;
        }

        SeparatedSyntaxList<ArgumentSyntax> arguments = argumentList.Arguments;

        SeparatedSyntaxList<ArgumentSyntax> newArguments =
            SyntaxFactory.SeparatedList(
                arguments.Select(NormalizeArgumentForSingleLine));

        ArgumentListSyntax newArgumentList =
            argumentList
                .WithOpenParenToken(
                    argumentList.OpenParenToken.WithTrailingTrivia())
                .WithArguments(newArguments)
                .WithCloseParenToken(
                    argumentList.CloseParenToken
                        .WithLeadingTrivia()
                        .WithTrailingTrivia(argumentList.CloseParenToken.TrailingTrivia));

        SyntaxNode newRoot = root.ReplaceNode(argumentList, newArgumentList);

        return document.WithSyntaxRoot(newRoot);
    }

    private static string GetLineIndentation(SourceText text, int position)
    {
        TextLine line = text.Lines.GetLineFromPosition(position);
        string lineText = line.ToString();

        return new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());
    }

    private static string GetIndentation(string line)
    {
        return new string(line.TakeWhile(char.IsWhiteSpace).ToArray());
    }

    private static string RemovePrefix(string text, string prefix)
    {
        return text.StartsWith(prefix, StringComparison.Ordinal)
            ? text.Substring(prefix.Length)
            : text;
    }

    private static ArgumentSyntax NormalizeArgumentForSingleLine(ArgumentSyntax argument)
    {
        return argument
            .WithoutTrivia()
            .NormalizeWhitespace();
    }

    private static ArgumentSyntax NormalizeArgumentForMultiLine(
        ArgumentSyntax argument,
        string newLine,
        string argumentIndent)
    {
        string text = argument.ToString();
        string[] lines = text.Split([newLine], StringSplitOptions.None);

        if (lines.Length <= 1)
        {
            return argument
                .WithoutLeadingTrivia()
                .WithoutTrailingTrivia();
        }

        string baseIndent =
            lines
                .Skip(1)
                .Where(line => line.Length > 0)
                .Select(GetIndentation)
                .DefaultIfEmpty("")
                .OrderBy(value => value.Length)
                .First();

        string adjustedText =
            string.Join(
                newLine,
                lines.Select((line, index) =>
                    index == 0
                        ? line
                        : argumentIndent + RemovePrefix(line, baseIndent)));

        return argument
            .WithExpression(SyntaxFactory.ParseExpression(adjustedText))
            .WithoutLeadingTrivia()
            .WithoutTrailingTrivia();
    }

}
