using kgh02017.CodeStyle.Analyzers.Readability;
using kgh02017.CodeStyle.CodeFixes.Readability;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Readability;

public sealed class PreferNamedArgumentsForLiteralsCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferNamedArgumentsForLiteralsAnalyzer, PreferNamedArgumentsForLiteralsCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task BoolLiteralArgument_WhenFixed_AddsParameterName()
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
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc(isEnabled: true);
                }

                public void TestFunc(bool isEnabled)
                {
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task NullLiteralArgument_WhenFixed_AddsParameterName()
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
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc(value: null);
                }

                public void TestFunc(int? value)
                {
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task BoolLiteralAsSecondArgument_WhenFixed_AddsCorrectParameterName()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc("abc", {|KGH1008:true|});
                }

                public void TestFunc(string keyword, bool isEnabled)
                {
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    TestFunc("abc", isEnabled: true);
                }

                public void TestFunc(string keyword, bool isEnabled)
                {
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
