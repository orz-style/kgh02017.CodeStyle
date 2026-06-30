using Microsoft.CodeAnalysis;

namespace kgh02017.CodeStyle.Analyzers;

internal static class DiagnosticDescriptorBuilder
{
    private const string HelpLinkBaseUri =
        "https://github.com/orz-style/kgh02017.CodeStyle/blob/main/src/kgh02017.CodeStyle/docs/rules/";

    public static DiagnosticDescriptor CreateRule(
        string id,
        string title,
        string message,
        string category,
        DiagnosticSeverity severity = DiagnosticSeverity.Warning,
        string? description = null)
    {
        return new DiagnosticDescriptor(
            id: id,
            title: title,
            messageFormat: message,
            category: category,
            defaultSeverity: severity,
            isEnabledByDefault: true,
            description: description,
            helpLinkUri: $"{HelpLinkBaseUri}{id}.md");
    }
}