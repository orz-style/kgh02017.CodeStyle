using kgh02017.CodeStyle.Analyzers.Nullability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Nullability;

public sealed class PreferIsNullAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferIsNullAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task EqualsNull_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test(string? value)
                {
                    if (value {|KGH1003:==|} null)
                    {
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NotEqualsNull_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test(string? value)
                {
                    if (value {|KGH1003:!=|} null)
                    {
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NullEquals_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test(string? value)
                {
                    if (null {|KGH1003:==|} value)
                    {
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NullNotEquals_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test(string? value)
                {
                    if (null {|KGH1003:!=|} value)
                    {
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task IsNull_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
           """
           public sealed class TestClass
           {
               public void Test(string? value)
               {
                   if (value is null)
                   {
                   }
               }
           }
           """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task IsNotNull_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
           """
           public sealed class TestClass
           {
               public void Test(string? value)
               {
                   if (value is not null)
                   {
                   }
               }
           }
           """;

        return VerifyAnalyzerAsync(source);
    }
}
