using System.Collections.Immutable;
using System.Composition;
using kgh02017.CodeStyle.Analyzers;
using kgh02017.CodeStyle.Analyzers.Readability;
using kgh02017.CodeStyle.CodeFixes.Readability;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Nullability;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferThrowIfNullCodeFixProvider))]
[Shared]
public sealed class PreferThrowIfNullCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferThrowIfNull];

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

        IfStatementSyntax? ifStatement =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<IfStatementSyntax>()
                .FirstOrDefault();

        if (ifStatement is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use ArgumentNullException.ThrowIfNull",
                cancellationToken =>
                    UseThrowIfNullAsync(context.Document, ifStatement, cancellationToken),
                nameof(PreferUsingDeclarationCodeFixProvider)),
           diagnostic);
    }

    private static async Task<Document> UseThrowIfNullAsync(
        Document document,
        IfStatementSyntax ifStatement,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (ifStatement.Condition is not IsPatternExpressionSyntax isPattern)
        {
            return document;
        }

        ExpressionSyntax valueExpression = isPattern.Expression;

        ExpressionStatementSyntax replacement =
            SyntaxFactory.ExpressionStatement(
                SyntaxFactory.InvocationExpression(
                    SyntaxFactory.MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        SyntaxFactory.IdentifierName("ArgumentNullException"),
                        SyntaxFactory.IdentifierName("ThrowIfNull")),
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(
                            SyntaxFactory.Argument(
                                valueExpression.WithoutTrivia())))))
            .WithTriviaFrom(ifStatement);

        SyntaxNode newRoot = root.ReplaceNode(ifStatement, replacement);

        return document.WithSyntaxRoot(newRoot);
    }
}
