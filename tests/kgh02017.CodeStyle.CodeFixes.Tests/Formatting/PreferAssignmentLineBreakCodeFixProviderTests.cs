using kgh02017.CodeStyle.Analyzers.Formatting;
using kgh02017.CodeStyle.CodeFixes.Formatting;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Formatting;

public sealed class PreferAssignmentLineBreakCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferAssignmentLineBreakAnalyzer, PreferAssignmentLineBreakCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task MultiLineInvocationAssignment_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
           """
            using System.Collections.Generic;
            using System.Linq;

            public sealed class TestClass
            {
                public void Test()
                {
                    IEnumerable<int> values {|KGH1007:=|} Enumerable.Range(1, 10)
                        .Where(x => x > 5)
                        .ToList();
                }
            }
            """;

        const string fixedSource =
           """
            using System.Collections.Generic;
            using System.Linq;

            public sealed class TestClass
            {
                public void Test()
                {
                    IEnumerable<int> values =
                        Enumerable.Range(1, 10)
                            .Where(x => x > 5)
                            .ToList();
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task MultiLineConditionalAssignment_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    bool condition = true;
                    string value {|KGH1007:=|} condition
                        ? "A"
                        : "B";
                }
            }
            """;

        const string fixedSource =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    bool condition = true;
                    string value =
                        condition
                            ? "A"
                            : "B";
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task MultiLineObjectInitializer_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
           """
            public class MyClass
            {
                public string Name { get; set; }
                public int Age { get; set; }
            }

            public sealed class TestClass
            {
                public void Test()
                {
                    MyClass value {|KGH1007:=|} new()
                    {
                        Name = "Test",
                        Age = 20,
                    };
                }
            }
            """;

        const string fixedSource =
           """
            public class MyClass
            {
                public string Name { get; set; }
                public int Age { get; set; }
            }

            public sealed class TestClass
            {
                public void Test()
                {
                    MyClass value =
                        new()
                        {
                            Name = "Test",
                            Age = 20,
                        };
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task MultiLineArrayInitializer_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int[] values {|KGH1007:=|} new[]
                    {
                        1,
                        2,
                    };
                }
            }
            """;

        const string fixedSource =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int[] values =
                        new[]
                        {
                            1,
                            2,
                        };
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
