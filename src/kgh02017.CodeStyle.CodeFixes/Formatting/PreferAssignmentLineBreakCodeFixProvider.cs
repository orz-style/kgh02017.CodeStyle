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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferAssignmentLineBreakCodeFixProvider))]
[Shared]
public sealed class PreferAssignmentLineBreakCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferAssignmentLineBreak];

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

        SyntaxNode? target =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .FirstOrDefault(node => node is EqualsValueClauseSyntax or AssignmentExpressionSyntax);

        if (target is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Places the assigned value on the following line",
                createChangedDocument: cancellationToken =>
                {
                    return target switch
                    {
                        EqualsValueClauseSyntax initializer =>
                            UseAssignmentLineBreakAsync(
                                context.Document,
                                initializer,
                                cancellationToken),

                        AssignmentExpressionSyntax assignment =>
                            UseAssignmentLineBreakAsync(
                                context.Document,
                                assignment,
                                cancellationToken),

                        _ => Task.FromResult(context.Document),
                    };
                },
                equivalenceKey: nameof(PreferAssignmentLineBreakCodeFixProvider)),
            diagnostic);
        }

    private static async Task<Document> UseAssignmentLineBreakAsync(
        Document document,
        EqualsValueClauseSyntax initializer,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        LocalDeclarationStatementSyntax? declaration =
            initializer.Ancestors()
                .OfType<LocalDeclarationStatementSyntax>()
                .FirstOrDefault();

        if (declaration is null)
        {
            return document;
        }

        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);
        string declarationIndent = GetIndentation(declaration.GetLeadingTrivia());
        string valueIndent = declarationIndent + "    ";

        SyntaxTriviaList equalsTrailingTrivia =
            SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(newLine), SyntaxFactory.Whitespace(valueIndent));

        ExpressionSyntax adjustedValue = AddContinuationIndent(initializer.Value, newLine);

        SyntaxToken equalsToken =
            SyntaxFactory.Token(
                SyntaxTriviaList.Empty,
                SyntaxKind.EqualsToken,
                equalsTrailingTrivia);

        EqualsValueClauseSyntax replacement =
            initializer
                .WithEqualsToken(equalsToken)
                .WithValue(adjustedValue.WithoutLeadingTrivia());

        SyntaxNode newRoot = root.ReplaceNode(initializer, replacement);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> UseAssignmentLineBreakAsync(
        Document document,
        AssignmentExpressionSyntax assignment,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        ExpressionSyntax right = assignment.Right;

        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);
        SourceText sourceText = await document.GetTextAsync(cancellationToken);

        string leftIndent = GetLineIndentation(sourceText, assignment.OperatorToken.SpanStart);
        string rightIndent = leftIndent + "    ";

        SyntaxTriviaList operatorTrailingTrivia =
            SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(newLine), SyntaxFactory.Whitespace(rightIndent));

        ExpressionSyntax adjustedValue = AddContinuationIndent(right, newLine);

        SyntaxToken operatorToken =
            SyntaxFactory.Token(
                SyntaxTriviaList.Empty,
                assignment.OperatorToken.Kind(),
                operatorTrailingTrivia);

        AssignmentExpressionSyntax replacement =
            assignment
                .WithOperatorToken(operatorToken)
                .WithRight(adjustedValue.WithoutLeadingTrivia());

        SyntaxNode newRoot = root.ReplaceNode(assignment, replacement);

        return document.WithSyntaxRoot(newRoot);
    }

    private static ExpressionSyntax AddContinuationIndent(ExpressionSyntax expression, string newLine)
    {
        string text = expression.WithoutLeadingTrivia().ToFullString();

        string adjusted = text.Replace(newLine, newLine + "    ");

        return SyntaxFactory.ParseExpression(adjusted).WithoutLeadingTrivia();
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

    private static string GetLineIndentation(SourceText sourceText, int position)
    {
        TextLine line = sourceText.Lines.GetLineFromPosition(position);
        string lineText = line.ToString();

        return new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());
    }
}
