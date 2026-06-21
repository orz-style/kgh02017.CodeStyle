using kgh02017.CodeStyle.Analyzers.Formatting;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class PreferConsistentMultilineArgumentsAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferConsistentMultilineArgumentsAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task Invocation_WhenMultipleArgumentsOnSameLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(1,
                                2, 3)|};
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenOneArgumentPerLine_DoesNotReportDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1,
                                  2,
                                  3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenSingleLineArgumentList_DoesNotReportDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
