using System.Collections.Immutable;
using System.Composition;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Readability;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferUsingDeclarationCodeFixProvider))]
[Shared]
public sealed class PreferUsingDeclarationCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferUsingDeclaration];

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

        UsingStatementSyntax? usingStatement =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<UsingStatementSyntax>()
                .FirstOrDefault();

        if (usingStatement is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                title: "Use a using declaration",
                createChangedDocument: cancellationToken =>
                    UseUsingDeclarationAsync(context.Document, usingStatement, cancellationToken),
                equivalenceKey: nameof(PreferUsingDeclarationCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> UseUsingDeclarationAsync(
        Document document,
        UsingStatementSyntax usingStatement,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        if (usingStatement.Declaration is null)
        {
            return document;
        }

        if (usingStatement.Statement is not BlockSyntax block)
        {
            return document;
        }

        SyntaxTriviaList leadingTrivia = usingStatement.GetLeadingTrivia();
        SyntaxTriviaList trailingTrivia = usingStatement.GetTrailingTrivia();

        LocalDeclarationStatementSyntax usingDeclaration =
            SyntaxFactory.LocalDeclarationStatement(usingStatement.Declaration)
                .WithUsingKeyword(
                    SyntaxFactory.Token(
                        leadingTrivia,
                        SyntaxKind.UsingKeyword,
                        SyntaxFactory.TriviaList(SyntaxFactory.Space)))
                .WithSemicolonToken(
                    SyntaxFactory.Token(
                        SyntaxFactory.TriviaList(),
                        SyntaxKind.SemicolonToken,
                        trailingTrivia));

        StatementSyntax[] bodyStatements =
            block.Statements.Select(statement => statement.WithLeadingTrivia(leadingTrivia)).ToArray();

        SyntaxList<StatementSyntax> replacementStatements =
            SyntaxFactory.List(
                new StatementSyntax[] { usingDeclaration }
                    .Concat(bodyStatements));

        SyntaxNode newRoot = root.ReplaceNode(usingStatement, replacementStatements);

        return document.WithSyntaxRoot(newRoot);
    }
}
