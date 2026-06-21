using kgh02017.CodeStyle.Analyzers.Formatting;
using kgh02017.CodeStyle.CodeFixes.Formatting;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Formatting;

public sealed class PreferConsistentMultilineParametersCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource, string equivalenceKey) =>
        VerifyCS<PreferConsistentMultilineParametersAnalyzer, PreferConsistentMultilineParametersCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource, equivalenceKey);

    [Fact]
    public Task Declaration_WhenFixedWithOneParameterPerLine_FormatsParameters()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum{|KGH1013:(int x,
                    int y, int z)|}
                {
                    return x + y + z;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum(
                    int x,
                    int y,
                    int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneParameterPerLine");
    }

    [Fact]
    public Task Declaration_WhenFixedWithSingleLineParameterList_FormatsParameters()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum{|KGH1013:(int x,
                    int y, int z)|}
                {
                    return x + y + z;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineParameterList");
    }

    [Fact]
    public Task Declaration_WhenParameterHasDefaultValue_PreservesDefaults()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum{|KGH1013:(int x,
                    int y = 0, int z = 0)|}
                {
                    return x + y + z;
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum(1, 2, 3);
                }

                public int Sum(
                    int x,
                    int y = 0,
                    int z = 0)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneParameterPerLine");
    }
}
