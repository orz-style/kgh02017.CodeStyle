using kgh02017.CodeStyle.Analyzers.Nullability;
using kgh02017.CodeStyle.CodeFixes.Nullability;

namespace kgh02017.CodeStyle.CodeFixes.Tests.Nullability;

public sealed class PreferThrowIfNullCodeFixProviderTests
{
    private static Task VerifyCodeFixAsync(string source, string fixedSource) =>
        VerifyCS<PreferThrowIfNullAnalyzer, PreferThrowIfNullCodeFixProvider>.VerifyCodeFixAsync(source, fixedSource);

    [Fact]
    public Task IfIsNullThrowArgumentNullException_WhenFixed_ReplacesWithThrowIfNull()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    {|KGH1009:if (value is null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }|}
                }
            }
            """;

        const string fixedSource =
            """
            using System;

            public sealed class TestClass
            {
                public void Test(string value)
                {
                    ArgumentNullException.ThrowIfNull(value);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }

#if false
    [Fact]
    public Task IfIsNullThrowArgumentNullExceptionWithThisMember_WhenFixed_ReplacesWithThrowIfNull()
    {
        const string source =
            """
            using System;

            public sealed class TestClass
            {
                private string? _value;

                public void Test()
                {
                    {|KGH1009:if (this._value is null)
                    {
                        throw new ArgumentNullException(nameof(_value));
                    }|}
                }
            }
            """;

        const string fixedSource =
            """
            using System;

            public sealed class TestClass
            {
                private string? _value;

                public void Test()
                {
                    ArgumentNullException.ThrowIfNull(this._value);
                }
            }
            """;

        return VerifyCodeFixAsync(source, fixedSource);
    }
#endif
}
