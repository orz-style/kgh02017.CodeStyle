using kgh02017.CodeStyle.Analyzers.Readability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Readability;

public sealed class PreferUsingDeclarationAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferUsingDeclarationAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task UsingStatement_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    {|KGH1005:using|} (var stream = new MemoryStream())
                    {
                        stream.WriteByte(0);
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task UsingDeclaration_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    using var stream = new MemoryStream();
                    stream.WriteByte(0);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
