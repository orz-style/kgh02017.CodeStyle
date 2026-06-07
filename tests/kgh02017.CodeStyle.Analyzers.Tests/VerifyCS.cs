using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace kgh02017.CodeStyle.Analyzers.Tests;

internal static class VerifyCS<TAnalyzer>
    where TAnalyzer : DiagnosticAnalyzer, new()
{
    public static DiagnosticResult Diagnostic()
    {
        return CSharpAnalyzerVerifier<TAnalyzer, DefaultVerifier>.Diagnostic();
    }

    public static Task VerifyAnalyzerAsync(string source)
    {
        var test =
            new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
            {
                TestCode = source,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            };

        return test.RunAsync();
    }

    public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
    {
        var test =
            new CSharpAnalyzerTest<TAnalyzer, DefaultVerifier>
            {
                TestCode = source,
                ReferenceAssemblies = ReferenceAssemblies.Net.Net90,
            };

        test.ExpectedDiagnostics.AddRange(expected);

        return test.RunAsync();
    }
}
