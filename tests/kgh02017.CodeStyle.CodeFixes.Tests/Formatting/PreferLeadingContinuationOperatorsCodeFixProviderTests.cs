using kgh02017.CodeStyle.Analyzers.Formatting;
using kgh02017.CodeStyle.CodeFixes.Formatting;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Formatting;

public sealed class PreferLeadingContinuationOperatorsCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource, string equivalenceKey) =>
        VerifyCS<PreferLeadingContinuationOperatorsAnalyzer, PreferLeadingContinuationOperatorsCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource, equivalenceKey);

    [Fact]
    public Task LogicalAndChain_WhenFixedWithLeadingOperators_FormatsEntireExpression()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a {|KGH1014:&&|}
                        b &&
                        c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a
                        && b
                        && c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseLeadingOperators");
    }

    [Fact]
    public Task LogicalAndChain_WhenFixedWithSingleLineExpression_FormatsEntireExpression()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a {|KGH1014:&&|}
                        b &&
                        c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a && b && c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineExpression");
    }

    [Fact]
    public Task LogicalOrChain_WhenFixedWithLeadingOperators_FormatsEntireExpression()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a {|KGH1014:|||}
                        b ||
                        c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c)
                {
                    if (a
                        || b
                        || c)
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseLeadingOperators");
    }

    [Fact]
    public Task LogicalMixedChain_WhenFixedWithLeadingOperators_FormatsEntireExpression()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c, bool d)
                {
                    if ((a && b) {|KGH1014:|||}
                        (c && d))
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c, bool d)
                {
                    if ((a && b)
                        || (c && d))
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseLeadingOperators");
    }

    [Fact]
    public Task LogicalMixedChain_WhenFixedWithSingeLineExpression_FormatsEntireExpression()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c, bool d)
                {
                    if ((a && b) {|KGH1014:|||}
                        (c && d))
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsEnabled(bool a, bool b, bool c, bool d)
                {
                    if ((a && b) || (c && d))
                    {
                        return true;
                    }

                    return false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineExpression");
    }

    [Fact]
    public Task Coalesce_WhenOperatorAtEndOfLineWhenFixedWithLeadingOperators_FormatsEntireExpression()
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

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void TestMethod(string? value)
                {
                    string Message = value
                        ?? string.Empty;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseLeadingOperators");
    }

    [Fact]
    public Task Coalesce_WhenOperatorAtEndOfLineWhenFixedWithSingleLineExpression_FormatsEntireExpression()
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

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void TestMethod(string? value)
                {
                    string Message = value ?? string.Empty;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineExpression");
    }

    [Fact]
    public Task Conditional_WhenFixedWithLeadingOperators_FormatsExpression()
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

        const string fixedSource =
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

        return VerifyCodeFixAsync(source, fixedSource, "UseLeadingOperators");
    }

    [Fact]
    public Task Conditional_WhenFixedWithSingleLineExpression_FormatsExpression()
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

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public bool IsPlusValue(int value)
                {
                    return value > 0 ? true : false;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineExpression");
    }
}
