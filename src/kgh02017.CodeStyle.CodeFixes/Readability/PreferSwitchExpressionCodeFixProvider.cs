using System.Collections.Immutable;
using System.Composition;
using System.Reflection.Emit;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Readability;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferSwitchExpressionCodeFixProvider))]
[Shared]
public sealed class PreferSwitchExpressionCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferSwitchExpression];

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

        SwitchStatementSyntax? switchStatement =
            root.FindToken(span.Start)
                .Parent?
                .AncestorsAndSelf()
                .OfType<SwitchStatementSyntax>()
                .FirstOrDefault();

        if (switchStatement is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use a switch expression",
                cancellationToken =>
                    UseSwitchExpressionAsync(context.Document, switchStatement, cancellationToken),
                nameof(PreferSwitchExpressionCodeFixProvider)),
            diagnostic);
    }

    private static async Task<Document> UseSwitchExpressionAsync(
        Document document,
        SwitchStatementSyntax switchStatement,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        List<SwitchExpressionArmSyntax> arms = [];
        foreach (SwitchSectionSyntax section in switchStatement.Sections)
        {
            var returnStatement = (ReturnStatementSyntax)section.Statements[0];

            foreach (SwitchLabelSyntax label in section.Labels)
            {

                SwitchExpressionArmSyntax arm;
                if (label is CaseSwitchLabelSyntax caseLabel)
                {
                    arm =
                        SyntaxFactory.SwitchExpressionArm(
                            SyntaxFactory.ConstantPattern(
                                caseLabel.Value.WithoutTrivia()),
                            returnStatement.Expression!.WithoutTrivia());
                }
                else
                {
                    arm =
                        SyntaxFactory.SwitchExpressionArm(
                            SyntaxFactory.DiscardPattern(),
                            returnStatement.Expression!.WithoutTrivia());
                }

                arms.Add(arm);
            }
        }

        SwitchExpressionSyntax switchExpression =
            SyntaxFactory.SwitchExpression(
                switchStatement.Expression.WithoutTrivia(),
                SyntaxFactory.SeparatedList(arms));

        ReturnStatementSyntax replacement =
            SyntaxFactory.ReturnStatement(switchExpression)
                .WithTriviaFrom(switchStatement)
                .WithAdditionalAnnotations(Formatter.Annotation);

        SyntaxNode newRoot = root.ReplaceNode(switchStatement, replacement);

        Document newDocument = document.WithSyntaxRoot(newRoot);

        return await CodeFixUtilities.FormatDocumentAsync(newDocument, cancellationToken);
    }
}
