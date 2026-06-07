using kgh02017.CodeStyle.Analyzers.Formatting;

namespace kgh02017.CodeStyle.Analyzers.Tests.Formatting;

public sealed class PreferAssignmentLineBreakAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferAssignmentLineBreakAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task MultiLineAssignment_WhenRightHandSideStartsOnSameLine_ReportsDiagnostic()
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task MultiLineConditionalExpression_WhenRightHandSideStartsOnSameLine_ReportsDiagnostic()
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task MultiLineAssignmentWithLineBreak_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task MultiLineConditionalExpressionWithLineBreak_WhenUsed_DoesNoReportDiagnostic()
    {
        const string source =
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

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task SingleLineAssignment_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
           """
            using System.Collections.Generic;
            using System.Linq;

            public sealed class TestClass
            {
                public void Test()
                {
                    IEnumerable<int> values = Enumerable.Range(1, 10);
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task MultiVariableDeclaration_WhenUsed_DoesNotReportDiagnostic()
    {
        const string source =
           """
            public sealed class TestClass
            {
                public void Test()
                {
                    int a = 1,
                        b = 2;
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

}
