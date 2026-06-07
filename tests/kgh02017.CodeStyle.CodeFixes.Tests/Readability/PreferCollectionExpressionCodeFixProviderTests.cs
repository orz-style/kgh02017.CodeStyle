using kgh02017.CodeStyle.Analyzers.Readability;
using kgh02017.CodeStyle.CodeFixes.Readability;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Readability;

public sealed class PreferCollectionExpressionCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferCollectionExpressionAnalyzer, PreferCollectionExpressionCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task EmptyListCreation_WhenFixed_UsesCollectionExpression()
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

        const string fixedSource =
            """
            using System.Collections.Generic;
            public sealed class TestClass
            {
                public void Test()
                {
                    List<string> names = [];
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task EmptyHashSetCreation_WhenFixed_UsesCollectionExpression()
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

        const string fixedSource =
            """
            using System.Collections.Generic;
            public sealed class TestClass
            {
                public void Test()
                {
                    HashSet<string> set = [];
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task EmptyDictionaryCreation_WhenFixed_UsesCollectionExpression()
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

        const string fixedSource =
            """
            using System.Collections.Generic;
            public sealed class TestClass
            {
                public void Test()
                {
                    Dictionary<string, int> dict = [];
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
