using System.Collections.Immutable;
using System.Composition;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Nullability;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferIsNullCodeFixProvider))]
[Shared]
public sealed class PreferIsNullCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferIsNull];

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

        BinaryExpressionSyntax? binaryExpression =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<BinaryExpressionSyntax>()
                .FirstOrDefault();

        if (binaryExpression is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use pattern matching null check",
                cancellationToken =>
                    UsePatternMatchingNullCheckAsync(context.Document, binaryExpression, cancellationToken),
                nameof(PreferIsNullCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> UsePatternMatchingNullCheckAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        CancellationToken cancellationToken)
    {
        ExpressionSyntax newExpression = CreateReplacementExpression(binaryExpression).WithTriviaFrom(binaryExpression);

        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        SyntaxNode newRoot = root.ReplaceNode(binaryExpression, newExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    private static IsPatternExpressionSyntax CreateReplacementExpression(BinaryExpressionSyntax binaryExpression)
    {
        bool isNotEquals = binaryExpression.IsKind(SyntaxKind.NotEqualsExpression);

        ExpressionSyntax targetExpression =
            IsNullLiteral(binaryExpression.Left)
                ? binaryExpression.Right
                : binaryExpression.Left;

        PatternSyntax pattern =
            isNotEquals
                ? SyntaxFactory.UnaryPattern(
                    SyntaxFactory.ConstantPattern(
                        SyntaxFactory.LiteralExpression(
                            SyntaxKind.NullLiteralExpression)))
                : SyntaxFactory.ConstantPattern(
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.NullLiteralExpression));

        return SyntaxFactory.IsPatternExpression(targetExpression.WithoutTrivia(), pattern);
    }

    private static bool IsNullLiteral(ExpressionSyntax expression)
    {
        return expression.IsKind(SyntaxKind.NullLiteralExpression);
    }

}
