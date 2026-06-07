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

        EqualsValueClauseSyntax? initializer =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<EqualsValueClauseSyntax>()
                .FirstOrDefault();

        if (initializer is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use assignment line break",
                cancellationToken =>
                    UseAssignmentLineBreakAsync(context.Document, initializer, cancellationToken),
                nameof(PreferAssignmentLineBreakCodeFixProvider)),
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
        string declarationIndent = declaration.GetLeadingTrivia().ToFullString();
        string valueIndent = declarationIndent + "    ";

        SyntaxTriviaList equalsTrailingTrivia =
            SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(newLine), SyntaxFactory.Whitespace(valueIndent));

        ExpressionSyntax adjustedValue = AddContinuationIndent(initializer.Value, newLine);

        EqualsValueClauseSyntax replacement =
            initializer
                .WithEqualsToken(initializer.EqualsToken.WithTrailingTrivia(equalsTrailingTrivia))
                .WithValue(adjustedValue);

        SyntaxNode newRoot = root.ReplaceNode(initializer, replacement);

        return document.WithSyntaxRoot(newRoot);
    }

    private static ExpressionSyntax AddContinuationIndent(ExpressionSyntax expression, string newLine)
    {
        string text = expression.ToFullString();
        string adjusted = text.Replace(newLine, newLine + "    ");

        return SyntaxFactory.ParseExpression(adjusted);
    }
}
