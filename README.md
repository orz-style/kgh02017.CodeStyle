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

#### Index

| Rule ID | Category | Rule Description |
| ------- | -------- | ---------------- |
| [KGH1001](src/kgh02017.CodeStyle/docs/rules/KGH1001.md) | Logging | Do not use interpolated strings in logger calls |
| [KGH1002](src/kgh02017.CodeStyle/docs/rules/KGH1002.md) | Strings | Specify StringComparison |
| [KGH1003](src/kgh02017.CodeStyle/docs/rules/KGH1003.md) | Nullability | Prefer is null |
| [KGH1004](src/kgh02017.CodeStyle/docs/rules/KGH1004.md) | Formatting | Line exceeds 120 characters |
| [KGH1005](src/kgh02017.CodeStyle/docs/rules/KGH1005.md) | Readability | Prefer using declaration |
| [KGH1006](src/kgh02017.CodeStyle/docs/rules/KGH1006.md) | Readability | Prefer switch expression |
| [KGH1007](src/kgh02017.CodeStyle/docs/rules/KGH1007.md) | Formatting | Prefer assignment line break |
| [KGH1008](src/kgh02017.CodeStyle/docs/rules/KGH1008.md) | Readability | Prefer named arguments for literal |
| [KGH1009](src/kgh02017.CodeStyle/docs/rules/KGH1009.md) | Nullability | Prefer ArgumentNullException.ThrowIfNull |
| [KGH1010](src/kgh02017.CodeStyle/docs/rules/KGH1010.md) | Logging | Prefer PascalCase logger template names |
| [KGH1011](src/kgh02017.CodeStyle/docs/rules/KGH1011.md) | Readability | Prefer collection expression |
| [KGH1012](src/kgh02017.CodeStyle/docs/rules/KGH1012.md) | Formatting | Prefer consistent multiline arguments |
| [KGH1013](src/kgh02017.CodeStyle/docs/rules/KGH1013.md) | Formatting | Prefer consistent multiline parameters |
| [KGH1014](src/kgh02017.CodeStyle/docs/rules/KGH1014.md) | Formatting | refer leading continuation operators |

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

- [kgh02017 Coding Style](src/kgh02017.CodeStyle/docs/CodingStyle.md)
- [.editorconfig](src/kgh02017.CodeStyle.Templates/templates/codestyle/.editorconfig)

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
