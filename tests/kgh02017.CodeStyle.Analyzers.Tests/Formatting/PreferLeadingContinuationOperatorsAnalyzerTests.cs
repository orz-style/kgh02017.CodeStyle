using kgh02017.CodeStyle.Analyzers.Formatting;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class PreferLeadingContinuationOperatorsAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferLeadingContinuationOperatorsAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task LogicalAnd_WhenOperatorAtEndOfLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b)
                {
                    if (a {|KGH1014:&&|}
                        b)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task LogicalAnd_WhenOperatorAtBeginningOfLine_DoesNotReportDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b)
                {
                    if (a
                        && b)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task LogicalOr_WhenOperatorAtEndOfLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b)
                {
                    if (a {|KGH1014:|||}
                        b)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Coalesce_WhenOperatorAtEndOfLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void TestMethod(string? value)
                {
                    string Message = value {|KGH1014:??|}
                        string.Empty;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Conditional_WhenQuestionTokenAtEndOfLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsPlusValue(int value)
                {
                    return value > 0 {|KGH1014:?|}
                        true
                        : false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Conditional_WhenTokensAtBeginningOfLine_DoesNotReportDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsPlusValue(int value)
                {
                    return value > 0
                        ? true
                        : false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Conditional_WhenColonTokenAtEndOfLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public bool IsPlusValue(int value)
                {
                    return value > 0
                        ? true {|KGH1014::|} false;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
