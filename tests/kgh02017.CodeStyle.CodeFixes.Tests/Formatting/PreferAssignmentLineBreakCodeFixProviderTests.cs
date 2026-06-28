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

    [Fact]
    public Task MultiLineAwaitExpression_WhenFixed_BreaksLineAfterAssignment()
    {
        const string source =
           """
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task TestAsync()
                {
                    var value {|KGH1007:=|} await Task.FromResult(
                        Enumerable.Range(1, 10)
                            .ToList());
                }
            }
            """;

        const string fixedSource =
           """
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task TestAsync()
                {
                    var value =
                        await Task.FromResult(
                            Enumerable.Range(1, 10)
                                .ToList());
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task MultiLineAwaitFluentInvocation_WhenDeclarationHasLeadingTrivia_UsesIndentationOnly()
    {
        const string source =
            """
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task TestAsync()
                {
                    var dbContext = new DbContext();

                    // Existing spools
                    var existingSpools {|KGH1007:=|} await dbContext.Spools
                        .Select(s => s.ShortCode)
                        .ToHashSetAsync();
                }
            }

            public sealed class DbContext
            {
                public IQueryable<Spool> Spools { get; } = new List<Spool>().AsQueryable();
            }

            public sealed class Spool
            {
                public string ShortCode { get; set; } = "";
            }

            public static class AsyncExtensions
            {
                public static Task<HashSet<T>> ToHashSetAsync<T>(
                    this IEnumerable<T> source) =>
                    Task.FromResult(source.ToHashSet());
            }
            """;

        const string fixedSource =
            """
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task TestAsync()
                {
                    var dbContext = new DbContext();

                    // Existing spools
                    var existingSpools =
                        await dbContext.Spools
                            .Select(s => s.ShortCode)
                            .ToHashSetAsync();
                }
            }

            public sealed class DbContext
            {
                public IQueryable<Spool> Spools { get; } = new List<Spool>().AsQueryable();
            }

            public sealed class Spool
            {
                public string ShortCode { get; set; } = "";
            }

            public static class AsyncExtensions
            {
                public static Task<HashSet<T>> ToHashSetAsync<T>(
                    this IEnumerable<T> source) =>
                    Task.FromResult(source.ToHashSet());
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Assignment_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
            """
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    int value = 0;
                    value {|KGH1007:=|} await GetValueAsync()
                        .ConfigureAwait(false);
                }

                private async Task<int> GetValueAsync()
                {
                    await Task.Delay(1000);
                    return 0;
                }
            }
            """;

        const string fixedSource =
            """
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    int value = 0;
                    value =
                        await GetValueAsync()
                            .ConfigureAwait(false);
                }

                private async Task<int> GetValueAsync()
                {
                    await Task.Delay(1000);
                    return 0;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task AddAssignment_WhenFixed_MovesRightHandSideToNextLine()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public event EventHandler Handler;

                public void Test()
                {
                    Handler {|KGH1007:+=|} (_, _) =>
                    {
                    };
                }
            }
            """;

        const string fixedSource =
            """
            using System;

            public sealed class TestClass
            {
                public event EventHandler Handler;

                public void Test()
                {
                    Handler +=
                        (_, _) =>
                        {
                        };
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task Assignment_WhenInsideObjectInitializer_UsesPropertyIndentation()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    var person =
                        new Person
                        {
                            Name {|KGH1007:=|} BuildName(
                                "Taro",
                                "Yamada"),
                        };
                }

                private string BuildName(string firstName, string lastName)
                {
                    return firstName + lastName;
                }

                private sealed class Person
                {
                    public string Name { get; set; } = "";
                }
            }
            """;

        const string fixedSource =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    var person =
                        new Person
                        {
                            Name =
                                BuildName(
                                    "Taro",
                                    "Yamada"),
                        };
                }

                private string BuildName(string firstName, string lastName)
                {
                    return firstName + lastName;
                }

                private sealed class Person
                {
                    public string Name { get; set; } = "";
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
