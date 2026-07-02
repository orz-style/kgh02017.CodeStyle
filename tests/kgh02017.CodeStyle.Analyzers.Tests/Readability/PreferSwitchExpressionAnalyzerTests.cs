using kgh02017.CodeStyle.Analyzers.Readability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Readability;

public sealed class PreferSwitchExpressionAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferSwitchExpressionAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task SwitchStatementReturningValues_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    Convert(1);
                }

                public string Convert(int value)
                {
                    {|KGH1006:switch|} (value)
                    {
                        case 1:
                            return "One";

                        case 2:
                            return "Two";

                        default:
                            return "Other";
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SwitchStatementWithMultipleStatements_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public string Convert(int value)
                {
                    switch (value)
                    {
                        case 1:
                            Console.WriteLine("One");
                            return "One";

                        default:
                            return "Other";
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SwitchExpression_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    Convert(1);
                }

                public string Convert(int value)
                {
                    return value switch
                    {
                        1 => "One",
                        2 => "Two",
                        _ => "Other",
                    };
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptySwitchStatement_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    Convert(1);
                }

                public string Convert(int value)
                {
                    switch (value)
                    {
                    }

                    return "Other";
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SwitchStatementWithMultipleLabels_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public string Convert(int value)
                {
                    {|KGH1006:switch|} (value)
                    {
                        case 1:
                        case 2:
                            return "Small";

                        default:
                            return "Other";
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SwitchStatement_WhenContainingThrow_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public string Convert(int value)
                {
                    switch (value)
                    {
                        case 1:
                            return "One";

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SwitchStatementWithBlockWrappedReturns_WhenUsed_ReportsDiagnostic()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public string Convert(int value)
                {
                    {|KGH1006:switch|} (value)
                    {
                        case 1:
                        {
                            return "One";
                        }

                        default:
                        {
                            return "Other";
                        }
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
