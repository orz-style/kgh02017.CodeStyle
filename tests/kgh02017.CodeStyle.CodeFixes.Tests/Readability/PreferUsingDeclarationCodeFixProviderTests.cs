using kgh02017.CodeStyle.Analyzers.Readability;
using kgh02017.CodeStyle.CodeFixes.Readability;
using kgh02017.CodeStyle.CodeFixes.Tests;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Readability;

public sealed class PreferUsingDeclarationCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferUsingDeclarationAnalyzer, PreferUsingDeclarationCodeFixProvider>
            .VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task UsingStatement_WhenFixed_ReplacesWithUsingDeclaration()
    {
        const string source =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    {|KGH1005:using|} (var stream = new MemoryStream())
                    {
                        stream.WriteByte(0);
                    }
                }
            }
            """;

        const string fixedSource =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    using var stream = new MemoryStream();
                    stream.WriteByte(0);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public Task UsingStatementWithMultipleStatements_WhenFixed_MovesStatementsOutsideUsingBlock()
    {
        const string source =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    {|KGH1005:using|} (var stream = new MemoryStream())
                    {
                        stream.WriteByte(0);
                        stream.WriteByte(1);
                    }
                }
            }
            """;

        const string fixedSource =
            """
            using System.IO;

            public sealed class TestClass
            {
                public void Test()
                {
                    using var stream = new MemoryStream();
                    stream.WriteByte(0);
                    stream.WriteByte(1);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
}
