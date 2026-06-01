# kgh02017.CodeStyle

> This project reflects my personal coding style and preferences.
> Some rules are intentionally opinionated.

Personal .NET coding style, project templates, Roslyn analyzers, and code fixes for modern C# development.

This repository contains:

* .NET project templates
* Coding style guidelines
* EditorConfig settings

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

## Installation

Install the template package:

```bash
dotnet new install kgh02017.CodeStyle.Templates
```

Create a new coding style configuration in an existing repository:

```bash
dotnet new kgh02017.codestyle
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
|   |
|   `-- kgh02017.CodeStyle.Templates
|
|-- tests
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
