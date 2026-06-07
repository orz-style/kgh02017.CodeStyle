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

#### Roslyn code fixes for supported style violations.

| Rule ID | CodeFix Provider  |
| ------- | ------------------|
| KGH1003 | PreferIsNullCodeFixProvider |
| KGH1005 | PreferUsingDeclarationCodeFixProvider |

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
