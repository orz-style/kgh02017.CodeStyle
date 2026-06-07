using kgh02017.CodeStyle.Analyzers.Readability;
using kgh02017.CodeStyle.CodeFixes.Readability;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Readability;

public sealed class PreferSwitchExpressionCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferSwitchExpressionAnalyzer, PreferSwitchExpressionCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task SwitchStatementReturningValues_WhenFixed_UsesSwitchExpression()
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

        const string fixedSource =
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
                        _ => "Other"
                    };
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task SwitchStatementWithMultipleLabels_WhenFixed_UsesSwitchExpression()
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

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public string Convert(int value)
                {
                    return value switch
                    {
                        1 => "Small",
                        2 => "Small",
                        _ => "Other"
                    };
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
