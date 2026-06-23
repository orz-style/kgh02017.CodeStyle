using kgh02017.CodeStyle.Analyzers.Formatting;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class PreferConsistentMultilineParametersAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferConsistentMultilineParametersAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task Declaration_WhenMultipleParametersOnSameLine_ReportsDiagnostic()
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Declaration_WhenOneParameterPerLine_DoesNotReportDiagnostic()
    {
        const string source =
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task Declaration_WhenSingleLineParameterList_DoesNotReportDiagnostic()
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
    public Task Constructor_WhenMultipleParametersOnSameLine_ReportsDiagnostic()
    {
        const string source =
           """
            public class Person
            {
               private string _last;
               private string _first;
               private int _age;

               public Person{|KGH1013:(string lastName,
                    string firstName, int age)|}
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
    public Task Declaration_WhenFirstParameterAreOnSameLineAsOpenParen_ReportsDiagnostic()
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
                    int y,
                    int z)|}
                {
                    return x + y + z;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
