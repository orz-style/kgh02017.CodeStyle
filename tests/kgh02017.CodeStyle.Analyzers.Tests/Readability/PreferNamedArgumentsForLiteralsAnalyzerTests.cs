using kgh02017.CodeStyle.Analyzers.Readability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Readability;

public sealed class PreferNamedArgumentsForLiteralsAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferNamedArgumentsForLiteralsAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task NullLiteralArgument_WhenNameIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc({|KGH1008:null|});
                }

                public void TestFunc(int? value)
                {
                    if (value is null)
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task TrueLiteralArgument_WhenNameIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc({|KGH1008:true|});
                }
                public void TestFunc(bool isEnabled)
                {
                    if (!isEnabled)
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task FalseLiteralArgument_WhenNameIsNotSpecified_ReportsDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc({|KGH1008:false|});
                }
                public void TestFunc(bool isEnabled)
                {
                    if (!isEnabled)
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NamedNullLiteralArgument_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc(value: null);
                }

                public void TestFunc(int? value)
                {
                    if (value is null)
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NamedBoolLiteralArgument_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc(isEnabled: true);
                }
                public void TestFunc(bool isEnabled)
                {
                    if (!isEnabled)
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task VariableArgument_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    string name = "taro";
                    TestFunc(name);
                }

                public void TestFunc(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task NamedStringLiteralArgument_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc(name: "taro");
                }

                public void TestFunc(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task StringLiteralArgument_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc("taro");
                }

                public void TestFunc(string name)
                {
                    if (string.IsNullOrEmpty(name))
                    {
                        return;
                    }
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
