using kgh02017.CodeStyle.Analyzers.Strings;

namespace kgh02017.CodeStyle.Analyzers.Tests.Strings;

public sealed class SpecifyStringComparisonAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<SpecifyStringComparisonAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task StartsWith_WhenStringComparisonIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.{|KGH1002:StartsWith|}("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task StartsWith_WhenStringComparisonIsSpecified_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.StartsWith("abc", StringComparison.Ordinal);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Contains_WhenStringComparisonIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.{|KGH1002:Contains|}("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Contains_WhenStringComparisonIsSpecified_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.Contains("abc", StringComparison.Ordinal);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EndsWith_WhenStringComparisonIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.{|KGH1002:EndsWith|}("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EndsWith_WhenStringComparisonIsSpecified_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    value.EndsWith("abc", StringComparison.Ordinal);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task CustomMethodNamedStartsWith_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class MyType
            {
                public bool StartsWith(string value)
                {
                    return true;
                }
            }

            public sealed class TestClass
            {
                public void Test(MyType value)
                {
                    value.StartsWith("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task CustomMethodNamedContains_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class MyType
            {
                public bool Contains(string value)
                {
                    return true;
                }
            }

            public sealed class TestClass
            {
                public void Test(MyType value)
                {
                    value.Contains("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task CustomMethodNamedEndsWith_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class MyType
            {
                public bool EndsWith(string value)
                {
                    return true;
                }
            }

            public sealed class TestClass
            {
                public void Test(MyType value)
                {
                    value.EndsWith("abc");
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
