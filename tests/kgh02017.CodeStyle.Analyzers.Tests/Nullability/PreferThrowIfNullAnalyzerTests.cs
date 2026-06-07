using kgh02017.CodeStyle.Analyzers.Nullability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Nullability;

public sealed class PreferThrowIfNullAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferThrowIfNullAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task IfIsNullThrowArgumentNullException_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    {|KGH1009:if (value is null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }|}
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task ThrowIfNull_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    ArgumentNullException.ThrowIfNull(value);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task IfIsNullThrowDifferentException_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    if (value is null)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task IfIsNullThrowArgumentNullExceptionWithoutNameof_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    if (value is null)
                    {
                        throw new ArgumentNullException("value");
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task IfIsNullThrowArgumentNullExceptionForDifferentVariable_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value, string other)
                {
                    if (value is null)
                    {
                        throw new ArgumentNullException(nameof(other));
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
