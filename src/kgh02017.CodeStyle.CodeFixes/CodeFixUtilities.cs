using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace kgh02017.CodeStyle.CodeFixes;

internal static class CodeFixUtilities
{
    public static async Task<string> GetNewLineAsync(
        Document document,
        CancellationToken cancellationToken)
    {
        SourceText text =
            await document.GetTextAsync(cancellationToken);

        string source =
            text.ToString();

        return source.Contains("\r\n")
            ? "\r\n"
            : "\n";
    }

    public static async Task<Document> NormalizeLineEndingsAsync(
        Document document,
        string newLine,
        CancellationToken cancellationToken)
    {
        SourceText text = await document.GetTextAsync(cancellationToken);

        string normalized =
            text.ToString()
                .Replace("\r\n", "\n")
                .Replace("\n", newLine);

        return document.WithText(SourceText.From(normalized, text.Encoding));
    }

    public static async Task<Document> FormatDocumentAsync(
        Document document,
        CancellationToken cancellationToken)
    {
        string newLine = await GetNewLineAsync(document, cancellationToken);

        Document formattedDocument =
            await Formatter.FormatAsync(
                document,
                Formatter.Annotation,
                cancellationToken: cancellationToken);

        return await NormalizeLineEndingsAsync(formattedDocument, newLine, cancellationToken);
    }
}
