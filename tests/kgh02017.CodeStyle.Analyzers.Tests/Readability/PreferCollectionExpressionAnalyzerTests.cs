using kgh02017.CodeStyle.Analyzers.Readability;

namespace kgh02017.CodeStyle.Analyzers.Tests.Readability;

public sealed class PreferCollectionExpressionAnalyzerTests
{
    private static Task VerifyAnalyzerAsync(string source) =>
        VerifyCS<PreferCollectionExpressionAnalyzer>.VerifyAnalyzerAsync(source);

    [Fact]
    public Task EmptyListCreation_WhenTypeIsApparent_ReportsDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    List<string> names = {|KGH1011:new()|};
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyHashSetCreation_WhenTypeIsApparent_ReportsDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    HashSet<string> set = {|KGH1011:new()|};
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyDictionaryCreation_WhenTypeIsApparent_ReportsDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    Dictionary<string, int> dict = {|KGH1011:new()|};
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyListCreation_WhenUsingVar_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    var names = new List<string>();
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyHashSetCreation_WhenUsingVar_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    var set = new HashSet<string>();
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyDictionaryCreation_WhenUsingVar_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System.Collections.Generic;

            public sealed class TestClass
            {
                public void Test()
                {
                    var dict = new Dictionary<string, int>();
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }

    [Fact]
    public Task EmptyImplicitObjectCreationForNonCollection_DoesNotReportDiagnostic()
    {
        const string source =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    MemoryStream stream = new();
                }
            }
            """;

        return VerifyAnalyzerAsync(source);
    }
}
