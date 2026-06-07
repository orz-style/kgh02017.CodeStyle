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

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferCollectionExpressionCodeFixProvider))]
[Shared]
public sealed class PreferCollectionExpressionCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferCollectionExpression];

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

        ImplicitObjectCreationExpressionSyntax? objectCreation =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<ImplicitObjectCreationExpressionSyntax>()
                .FirstOrDefault();

        if (objectCreation is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use collection expression",
                cancellationToken =>
                    UseCollectionExpressionAsync(context.Document, objectCreation, cancellationToken),
                nameof(PreferCollectionExpressionCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> UseCollectionExpressionAsync(
        Document document,
        ImplicitObjectCreationExpressionSyntax objectCreation,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        CollectionExpressionSyntax replacement =
            SyntaxFactory.CollectionExpression().WithTriviaFrom(objectCreation);

        SyntaxNode newRoot = root.ReplaceNode(objectCreation, replacement);

        return document.WithSyntaxRoot(newRoot);
    }
}
