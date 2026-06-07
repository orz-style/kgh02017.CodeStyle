using kgh02017.CodeStyle.Analyzers.Formatting;
using Microsoft.CodeAnalysis.Testing;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class MaximumLineLengthAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(
        string source,
        params DiagnosticResult[] expected) =>
            VerifyCS<MaximumLineLengthAnalyzer>.VerifyAnalyzerAsync(source, expected);

    [Fact]
    public Task LineLongerThan120Characters_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                private const string Value = "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890";
            }
            """;

        DiagnosticResult expected =
            VerifyCS<MaximumLineLengthAnalyzer>
                .Diagnostic()
                .WithLocation(3, 1);

        return VerifyAnalyzerAsync(source, expected);
    }

    [Fact]
    public Task LineNotLongerThan120Characters_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                private const string Value = "This line length is less than 120 characters.";
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
