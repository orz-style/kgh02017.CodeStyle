using kgh02017.CodeStyle.Analyzers.Formatting;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class PreferConsistentMultilineArgumentsAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferConsistentMultilineArgumentsAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task Invocation_WhenMultipleArgumentsOnSameLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int sum = Sum{|KGH1012:(1,
                                2, 3)|};
                }

                public int Sum(int x, int y, int z)
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenOneArgumentPerLine_DoesNotReportDiagnostic()
    {
        const string source =
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenSingleLineArgumentList_DoesNotReportDiagnostic()
    {
        const string source =
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task ObjectCreation_WhenMultipleArgumentsOnSameLine_ReportsDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    var p = new Person{|KGH1012:("Taro",
                        "Yamada", 25)|};
                }
            }

            public class Person
            {
               private string _last;
               private string _first;
               private int _age;

               public Person(string lastName, string firstName, int age)
               {
                  _last = lastName;
                  _first = firstName;
                  _age = age;
               }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenFirstArgumentIsOnSameLineAsOpenParen_ReportsDiagnostic()
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Invocation_WhenSingleMultilineArgumentStartsOnOpenParenLine_ReportsDiagnostic()
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

        return VerifyAnalyzerAsync(source);
    }
}
