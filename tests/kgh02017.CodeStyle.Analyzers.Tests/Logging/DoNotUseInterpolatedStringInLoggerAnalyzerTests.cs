using kgh02017.CodeStyle.Analyzers.Logging;

namespace kgh02017.CodeStyle.Analyzers.Tests.Logging;

public sealed class DoNotUseInterpolatedStringInLoggerAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<DoNotUseInterpolatedStringInLoggerAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task LogInformation_WhenInterpolatedStringIsUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class Logger
            {
                public void LogInformation(string message)
                {
                }
            }

            public sealed class TestClass
            {
                private readonly Logger _logger;

                public void Test()
                {
                    _logger.LogInformation({|KGH1001:$"Hello"|});
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
