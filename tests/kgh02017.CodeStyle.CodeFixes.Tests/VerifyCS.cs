using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace kgh02017.CodeStyle.CodeFixes.Tests;

internal static class VerifyCS<TAnalyzer, TCodeFix>
    where TAnalyzer : DiagnosticAnalyzer, new()
    where TCodeFix : CodeFixProvider, new()
{
    public static Task VerifyCodeFixAsync(string source, string fixedSource)
    {
        var test =
            new CSharpCodeFixTest<TAnalyzer, TCodeFix, DefaultVerifier>
            {
                TestCode = source,
                FixedCode = fixedSource,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            };

        return test.RunAsync();
    }
}
