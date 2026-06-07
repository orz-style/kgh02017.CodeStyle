using kgh02017.CodeStyle.Analyzers.Logging;

namespace kgh02017.CodeStyle.Analyzers.Tests.Logging;

public sealed class PreferPascalCaseLoggerTemplateNamesAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferPascalCaseLoggerTemplateNamesAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task LoggerTemplateName_WhenCamelCase_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class Logger
            {
                public void LogInformation(string message, object value)
                {
                }
            }

            public sealed class TestClass
            {
                private readonly Logger _logger = new();

                public void Test(string shortCode)
                {
                    _logger.LogInformation({|KGH1010:"Updating spool {shortCode}."|}, shortCode);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task LoggerTemplateName_WhenPascalCase_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class Logger
            {
                public void LogInformation(string message, object value)
                {
                }
            }

            public sealed class TestClass
            {
                private readonly Logger _logger = new();

                public void Test(string shortCode)
                {
                    _logger.LogInformation("Updating spool {ShortCode}.", shortCode);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
