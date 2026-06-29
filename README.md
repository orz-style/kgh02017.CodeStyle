# kgh02017.CodeStyle

> This project reflects my personal coding style and preferences.
> Some rules are intentionally opinionated.

Personal .NET coding style, project templates, Roslyn analyzers, and code fixes for modern C# development.

This repository contains:

* .NET project templates
* Coding style guidelines
* EditorConfig settings
* Roslyn analyzers and code fixes

The goal is to provide a consistent and opinionated development experience across personal .NET projects.

## Requirements

- .NET SDK 9.0 or later
- C# 12 or later

## Example (KGH1007)

Bad

```csharp
IEnumerable<int> values = Enumerable.Range(1, 10)
    .Where(x => x > 5)
    .ToList();
```

Good

```csharp
IEnumerable<int> values =
    Enumerable.Range(1, 10)
        .Where(x => x > 5)
        .ToList();
```

## Packages

### kgh02017.CodeStyle.Templates

Project templates for new .NET solutions.

Currently includes:

* `.editorconfig`
* `Directory.Build.props`
* `Directory.Packages.props`
* `CodeStyle.md`
* `.gitignore`

### kgh02017.CodeStyle

Combined Roslyn analyzers and code fixes package.

Includes all analyzers and supported code fixes.

#### Roslyn analyzers for coding style rules.

| Rule ID | Category | Severity | Rule Description |
| ------- | -------- | -------- | ---------------- |
| KGH1001 | Logging | Warning | Do not use interpolated string in logger calls. Use structured logging instead |
| KGH1002 | Strings | Warning | Specify StringComparison explicitly |
| KGH1003 | Nullability | Warning | Use 'is null' or 'is not null' instead of equality operators |
| KGH1004 | Formatting | Warning | Wrap the text so that line length is less than 120 characters |
| KGH1005 | Readability | Warning | Use a using declaration instead of a using statement |
| KGH1006 | Readability | Warning | Use a switch expression instead of a switch statement |
| KGH1007 | Formatting | Warning | Place a line break after the assignment operator |
| KGH1008 | Readability | Warning | Use named arguments for null and boolean literal arguments |
| KGH1009 | Nullability | Warning | Use ArgumentNullException.ThrowIfNull instead of throwing ArgumentNullException manually |
| KGH1010 | Logging | Warning | Use PascalCase for structured logging placeholder names |
| KGH1011 | Readability | Warning | Use a collection expression when the collection type is apparent |
| KGH1012 | Formatting | Warning | Use either a single-line argument list or one argument per line |
| KGH1013 | Formatting | Warning | Use either a single-line parameter list or one parameter per line |
| KGH1014 | Formatting | Warning | Place binary operators at the beginning of the continued line |

#### Roslyn code fixes for supported style violations.

| Rule ID | CodeFix Provider  |
| ------- | ------------------|
| KGH1003 | PreferIsNullCodeFixProvider |
| KGH1005 | PreferUsingDeclarationCodeFixProvider |
| KGH1006 | PreferSwitchExpressionCodeFixProvider |
| KGH1007 | PreferAssignmentLineBreakCodeFixProvider |
| KGH1008 | PreferNamedArgumentsForLiteralsCodeFixProvider |
| KGH1009 | PreferThrowIfNullCodeFixProvider |
| KGH1011 | PreferCollectionExpressionCodeFixProvider |
| KGH1012 | PreferConsistentMultilineArgumentsCodeFixProvider |
| KGH1013 | PreferConsistentMultilineParametersCodeFixProvider |
| KGH1014 | PreferLeadingContinuationOperatorsCodeFixProvider |

## Installation

Install the template package:

```bash
dotnet new install kgh02017.CodeStyle.Templates
```

Create a new coding style configuration in an existing repository:

```bash
dotnet new kgh02017.codestyle
```

Install the analyzers and code fixes package:

```bash
dotnet add package kgh02017.CodeStyle
```

## Coding Style

The coding conventions are documented in:

```text
CodeStyle.md
```

Key principles:

* Readability over cleverness
* Consistent formatting
* Modern C# where it improves clarity
* Minimal ceremony
* Structured logging
* Nullable reference types enabled

### Details

- [.editorconfig](/src/kgh02017.CodeStyle.Templates/templates/codestyle/.editorconfig)
- [CodeStyle.md](/src/kgh02017.CodeStyle.Templates/templates/codestyle/CodeStyle.md)

## Repository Structure

```text
kgh02017.CodeStyle
|
|-- src
|   |-- kgh02017.CodeStyle.Templates
|   |-- kgh02017.CodeStyle.Analyzers
|   `-- kgh02017.CodeStyle.CodeFixes
|
|-- tests
|   |-- kgh02017.CodeStyle.Analyzers.Tests
|   `-- kgh02017.CodeStyle.CodeFixes.Tests
|
|-- .editorconfig
|-- Directory.Build.props
|-- Directory.Packages.props
`-- CodeStyle.md
```

## License

Licensed under the MIT License.

## Author

Taku Izumi
