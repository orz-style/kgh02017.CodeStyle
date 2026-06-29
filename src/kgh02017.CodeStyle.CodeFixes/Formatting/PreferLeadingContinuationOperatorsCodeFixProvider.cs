using System.Collections.Immutable;
using System.Composition;
using System.Linq.Expressions;
using kgh02017.CodeStyle.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes.Formatting;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferLeadingContinuationOperatorsCodeFixProvider))]
[Shared]
public sealed class PreferLeadingContinuationOperatorsCodeFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => [DiagnosticIds.PreferLeadingContinuationOperators];

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
                .FirstOrDefault(node => node is BinaryExpressionSyntax or ConditionalExpressionSyntax);

        if (target is null)
        {
            return;
        }

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use leading operators",
                cancellationToken =>
                {
                    return target switch
                    {
                        BinaryExpressionSyntax binaryExpression =>
                            UseLeadingOperatorsAsync(context.Document, binaryExpression, cancellationToken),

                        ConditionalExpressionSyntax conditionalExpression =>
                            UseLeadingOperatorsAsync(context.Document, conditionalExpression, cancellationToken),

                        _ => Task.FromResult(context.Document),
                    };
                },
                "UseLeadingOperators"),
            diagnostic);

        context.RegisterCodeFix(
            CodeAction.Create(
                "Use single-line expression",
                cancellationToken =>
                {
                    return target switch
                    {
                        BinaryExpressionSyntax binaryExpression =>
                            UseSingleLineExpressionAsync(context.Document, binaryExpression, cancellationToken),

                        ConditionalExpressionSyntax conditionalExpression =>
                            UseSingleLineExpressionAsync(context.Document, conditionalExpression, cancellationToken),

                        _ => Task.FromResult(context.Document),
                    };
                },
                "UseSingleLineExpression"),
            diagnostic);
    }

    private static async Task<Document> UseLeadingOperatorsAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        SourceText sourceText = await document.GetTextAsync(cancellationToken);
        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);

        BinaryExpressionSyntax rootExpression = binaryExpression;

        while (rootExpression.Parent is BinaryExpressionSyntax parent
            && parent.Kind() == rootExpression.Kind())
        {
            rootExpression = parent;
        }

        string baseIndent = GetLineIndentation(sourceText, rootExpression.SpanStart);
        string operandIndent = baseIndent + "    ";

        SyntaxTriviaList operatorLeadingTrivia =
            [
                SyntaxFactory.EndOfLine(newLine),
                SyntaxFactory.Whitespace(operandIndent),
            ];

        SyntaxTriviaList operatorTrailingTrivia =
            [
                SyntaxFactory.Space,
            ];

        List<ExpressionSyntax> operandList = [];
        Flatten(rootExpression, rootExpression.Kind(), operandList);

        SyntaxToken CreateLeadingOperatorToken() =>
            SyntaxFactory.Token(
                operatorLeadingTrivia,
                rootExpression.OperatorToken.Kind(),
                operatorTrailingTrivia);

        BinaryExpressionSyntax newRootExpression =
            Build(
                rootExpression.Kind(),
                operandList,
                CreateLeadingOperatorToken);

        SyntaxNode newRoot = root.ReplaceNode(rootExpression, newRootExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> UseLeadingOperatorsAsync(
        Document document,
        ConditionalExpressionSyntax conditionalExpression,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        SourceText sourceText = await document.GetTextAsync(cancellationToken);
        string newLine = await CodeFixUtilities.GetNewLineAsync(document, cancellationToken);

        string baseIndent = GetLineIndentation(sourceText, conditionalExpression.SpanStart);
        string operandIndent = baseIndent + "    ";

        SyntaxTriviaList leadingTrivia =
            SyntaxFactory.TriviaList(SyntaxFactory.EndOfLine(newLine), SyntaxFactory.Whitespace(operandIndent));
        SyntaxTriviaList trailingTrivia = SyntaxFactory.TriviaList(SyntaxFactory.Space);

        SyntaxToken questionToken = conditionalExpression.QuestionToken;
        SyntaxToken colonToken = conditionalExpression.ColonToken;

        if (IsTokenAtEndOfLine(
                conditionalExpression.Condition,
                conditionalExpression.QuestionToken))
        {
            questionToken =
                SyntaxFactory.Token(
                    leadingTrivia,
                    SyntaxKind.QuestionToken,
                    trailingTrivia);
        }

        if (IsTokenAtEndOfLine(
                conditionalExpression.WhenTrue,
                conditionalExpression.ColonToken))
        {
            colonToken =
                SyntaxFactory.Token(
                    leadingTrivia,
                    SyntaxKind.ColonToken,
                    trailingTrivia);
        }

        ConditionalExpressionSyntax newConditionalExpression =
            conditionalExpression
                .WithQuestionToken(questionToken)
                .WithWhenTrue(conditionalExpression.WhenTrue.WithoutLeadingTrivia().WithoutTrailingTrivia())
                .WithColonToken(colonToken)
                .WithWhenFalse(conditionalExpression.WhenFalse.WithoutLeadingTrivia());

        SyntaxNode newRoot = root.ReplaceNode(conditionalExpression, newConditionalExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> UseSingleLineExpressionAsync(
        Document document,
        BinaryExpressionSyntax binaryExpression,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        BinaryExpressionSyntax rootExpression = binaryExpression;

        while (rootExpression.Parent is BinaryExpressionSyntax parent
            && parent.Kind() == rootExpression.Kind())
        {
            rootExpression = parent;
        }

        List<ExpressionSyntax> operandList = [];
        Flatten(rootExpression, rootExpression.Kind(), operandList);

        SyntaxToken CreateSingleLineOperatorToken() =>
            SyntaxFactory.Token(
                SyntaxFactory.TriviaList(SyntaxFactory.Space),
                rootExpression.OperatorToken.Kind(),
                SyntaxFactory.TriviaList(SyntaxFactory.Space));

        BinaryExpressionSyntax newRootExpression =
            Build(
                rootExpression.Kind(),
                operandList,
                CreateSingleLineOperatorToken);

        SyntaxNode newRoot = root.ReplaceNode(rootExpression, newRootExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    private static async Task<Document> UseSingleLineExpressionAsync(
        Document document,
        ConditionalExpressionSyntax conditionalExpression,
        CancellationToken cancellationToken)
    {
        SyntaxNode? root = await document.GetSyntaxRootAsync(cancellationToken);

        if (root is null)
        {
            return document;
        }

        SyntaxTriviaList spaceTrivia = SyntaxFactory.TriviaList(SyntaxFactory.Space);
        SyntaxToken questionToken = SyntaxFactory.Token(spaceTrivia, SyntaxKind.QuestionToken, spaceTrivia);
        SyntaxToken colonToken = SyntaxFactory.Token(spaceTrivia, SyntaxKind.ColonToken, spaceTrivia);

        ConditionalExpressionSyntax newConditionalExpression =
            conditionalExpression
                .WithCondition(conditionalExpression.Condition.WithoutLeadingTrivia().WithoutTrailingTrivia())
                .WithQuestionToken(questionToken)
                .WithWhenTrue(conditionalExpression.WhenTrue.WithoutLeadingTrivia().WithoutTrailingTrivia())
                .WithColonToken(colonToken)
                .WithWhenFalse(conditionalExpression.WhenFalse.WithoutLeadingTrivia()).WithoutTrailingTrivia()
                .WithTrailingTrivia(conditionalExpression.GetTrailingTrivia());

        SyntaxNode newRoot = root.ReplaceNode(conditionalExpression, newConditionalExpression);

        return document.WithSyntaxRoot(newRoot);
    }

    private static void Flatten(ExpressionSyntax expression, SyntaxKind kind, List<ExpressionSyntax> operands)
    {
        if (expression is BinaryExpressionSyntax binary
            && binary.Kind() == kind)
        {
            Flatten(binary.Left, kind, operands);
            Flatten(binary.Right, kind, operands);
        }
        else
        {
            operands.Add(expression);
        }
    }

    private static string GetLineIndentation(SourceText text, int position)
    {
        TextLine line = text.Lines.GetLineFromPosition(position);
        string lineText = line.ToString();

        return new string(lineText.TakeWhile(char.IsWhiteSpace).ToArray());
    }

    private static BinaryExpressionSyntax Build(
        SyntaxKind kind,
        List<ExpressionSyntax> operands,
        Func<SyntaxToken> createOperatorToken)
    {
        ExpressionSyntax expression =
            operands[0]
                .WithoutLeadingTrivia()
                .WithoutTrailingTrivia();

        for (int i = 1; i < operands.Count; i++)
        {
            expression =
                SyntaxFactory.BinaryExpression(
                    kind,
                    expression,
                    createOperatorToken(),
                    operands[i]
                        .WithoutLeadingTrivia()
                        .WithoutTrailingTrivia());
        }

        return (BinaryExpressionSyntax)expression;
    }

    private static bool IsTokenAtEndOfLine(ExpressionSyntax leftExpression, SyntaxToken operatorToken)
    {
        int leftLine =
            leftExpression.GetLocation()
                .GetLineSpan()
                .EndLinePosition
                .Line;

        int operatorLine =
            operatorToken.GetLocation()
                .GetLineSpan()
                .StartLinePosition
                .Line;

        return leftLine == operatorLine;
    }
}