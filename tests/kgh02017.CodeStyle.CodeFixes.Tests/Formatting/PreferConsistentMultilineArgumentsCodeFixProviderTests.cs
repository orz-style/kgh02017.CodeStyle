using kgh02017.CodeStyle.Analyzers.Formatting;
using kgh02017.CodeStyle.CodeFixes.Formatting;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Formatting;

public sealed class PreferConsistentMultilineArgumentsCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource, string equivalenceKey) =>
        VerifyCS<PreferConsistentMultilineArgumentsAnalyzer, PreferConsistentMultilineArgumentsCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource, equivalenceKey);

    [Fact]
    public Task Invocation_WhenFixedWithOneArgumentPerLine_FormatsArguments()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(1, 2,
                                            3)|};
                }

                public int Sum(int x, int y, int z)
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
                    int sum = Sum(
                        1,
                        2,
                        3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenFixedWithSingleLineArgumentList_FormatsArguments()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(1, 2,
                                            3)|};
                }

                public int Sum(int x, int y, int z)
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

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineArgumentList");
    }

    [Fact]
    public Task Invocation_WhenNestedInAwaitStatement_UsesStatementIndentation()
    {
        const string source =
            """
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    if (true)
                    {
                        await HeavyTask{|KGH1012:("test", 10,
                            true)|};
                    }
                }

                public async Task HeavyTask(string code, int weight, bool isEnabled)
                {
                    await Task.Delay(1000);
                    await Task.Delay(1000);
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
                    if (true)
                    {
                        await HeavyTask(
                            "test",
                            10,
                            true);
                    }
                }

                public async Task HeavyTask(string code, int weight, bool isEnabled)
                {
                    await Task.Delay(1000);
                    await Task.Delay(1000);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenNestedInAwaitStatement_FormatsArguments()
    {
        const string source =
            """
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    if (true)
                    {
                        await HeavyTask{|KGH1012:("test", 10,
                            true)|};
                    }
                }

                public async Task HeavyTask(string code, int weight, bool isEnabled)
                {
                    await Task.Delay(1000);
                    await Task.Delay(1000);
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
                    if (true)
                    {
                        await HeavyTask("test", 10, true);
                    }
                }

                public async Task HeavyTask(string code, int weight, bool isEnabled)
                {
                    await Task.Delay(1000);
                    await Task.Delay(1000);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineArgumentList");
    }

    [Fact]
    public Task Invocation_WhenArgumentsAreNamed_PreservesNames()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(x: 1,
                        y: 2,z: 3)|};
                }

                public int Sum(int x, int y, int z)
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
                    int sum = Sum(x: 1, y: 2, z: 3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineArgumentList");
    }

    [Fact]
    public Task Invocation_WhenFirstArgumentIsOnSameLineAsOpenParen_FormatsParameters()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(1,
                        2,
                        3)|};
                }

                public int Sum(int x, int y, int z)
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
                    int sum = Sum(
                        1,
                        2,
                        3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenCallIsIndentedContinuation_UsesCallLineIndentation()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum =
                        Sum{|KGH1012:(1, 2,
                            3)|};
                }

                public int Sum(int x, int y, int z)
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
                    int sum =
                        Sum(
                            1,
                            2,
                            3);
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenArgumentIsMultilineLambda_IndentsLambdaBody()
    {
        const string source =
            """
            public sealed class TestClass
            {
                public void Test()
                {
                    Run{|KGH1012:(() =>
                    {
                        int value = 1;
                    }, 10)|};
                }

                public void Run(System.Action action, int value)
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
                    Run(
                        () =>
                        {
                            int value = 1;
                        },
                        10);
                }

                public void Run(System.Action action, int value)
                {
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenMultilineLamdaArgumentStartsOnOpenParenLine_FormatsOneArgumentPerLine()
    {
        const string source =
           """
            using System;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    await Task.Run{|KGH1012:(() =>
                    {
                        Console.WriteLine("Hello");
                    })|};
                }
            }
            """;

        const string fixedSource =
            """
            using System;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    await Task.Run(
                        () =>
                        {
                            Console.WriteLine("Hello");
                        });
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseOneArgumentPerLine");
    }

    [Fact]
    public Task Invocation_WhenMultilineLamdaArgumentStartsOnOpenParenLine_FormatsSingleLineArguments()
    {
        const string source =
           """
            using System;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    await Task.Run{|KGH1012:(() =>
                        Console.WriteLine("Hello"))|};
                }
            }
            """;

        const string fixedSource =
            """
            using System;
            using System.Threading.Tasks;

            public sealed class TestClass
            {
                public async Task Test()
                {
                    await Task.Run(() => Console.WriteLine("Hello"));
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource, "UseSingleLineArgumentList");
    }
}
