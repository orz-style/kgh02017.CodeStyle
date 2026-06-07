using kgh02017.CodeStyle.Analyzers.Nullability;
using kgh02017.CodeStyle.CodeFixes.Nullability;
using kgh02017.CodeStyle.CodeFixes.Tests;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Nullability;

public sealed class PreferIsNullCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferIsNullAnalyzer, PreferIsNullCodeFixProvider>.VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task EqualsNull_WhenFixed_ReplacesWithIsNull()
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

        const string fixedSource =
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

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task NotEqualsNull_WhenFixed_ReplacesWithIsNotNull()
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

        const string fixedSource =
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

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task NullEquals_WhenFixed_ReplacesWithIsNull()
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

        const string fixedSource =
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

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task NullNotEquals_WhenFixed_ReplacesWithIsNotNull()
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

        const string fixedSource =
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

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
