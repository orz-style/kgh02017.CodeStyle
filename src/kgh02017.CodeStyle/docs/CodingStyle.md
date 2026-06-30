# kgh02017 Coding Style [2026-06]

This document describes the kgh02017 coding style.

## Core Policy

- Follow modern C# style where it improves readability.
- Prefer code that clearly shows structure.
- Avoid rules that only add ceremony.
- Keep code readable for the future maintainer.
- Use `.editorconfig` for enforceable style rules.
- Use this document for rules that cannot be expressed well in `.editorconfig`.

## Line Length

- Use 120 characters as the normal line length guideline.
- Keep short expressions on one line.
- If an expression exceeds roughly 120 characters, prefer breaking it into multiple lines.
- Do not wrap only to satisfy old 80-column conventions.

## Assignment and Line Breaks

Short assignments are written on one line.

```csharp
var name = spool.Name;
```

If the right-hand side is long or structurally complex, break after `=` and indent the right-hand side by one level.

```csharp
var locationName =
    await dbContext.Locations
        .AsNoTracking()
        .Where(location => location.ShortCode == shortCode)
        .Select(location => location.Name)
        .FirstOrDefaultAsync();
```

Do not break after `=` for trivial expressions.

```csharp
var count = 10;
```

## LINQ and Fluent Chains

Short chains may be written on one line.

```csharp
var name = spools.First().Name;
```

For longer chains, use one method call per line. Place each chained call on its own line with a leading dot.

```csharp
var locationName =
    await dbContext.Locations
        .AsNoTracking()
        .Where(location => location.ShortCode == shortCode)
        .Select(location => location.Name)
        .FirstOrDefaultAsync();
```

Do not vertically align chained calls with the receiver expression.

## Object Initializers

Use object initializers.

If an initializer fits cleanly on one line, one line is allowed.

```csharp
var location = new Location { Name = name };
```

For multiple properties, break after `=`.

```csharp
var location =
    new Location
    {
        Name = name,
        ShortCode = shortCode,
    };
```

Use trailing commas in multiline initializers.

Use `new()` when the type is clear.

```csharp
Location location =
    new()
    {
        Name = name,
        ShortCode = shortCode,
    };
```

## Collection Expressions and Initializers

Use collection expressions where safe and readable.

```csharp
string[] extensions = [".stl", ".3mf", ".obj"];
```

For multiline collection expressions, place `[` immediately after the assignment line, without an extra indentation level.

```csharp
List<string> materials =
[
    "PLA",
    "PETG",
    "TPU",
];
```

Use trailing commas in multiline collections.

## Method Declarations and Calls

Keep method declarations and calls on one line when they fit within the line length guideline.

```csharp
await UpdateSpoolAsync(shortCode, weight, locationShortCode);
```

When a declaration or call is multiline, use one parameter or argument per line.

```csharp
await UpdateSpoolAsync(
    shortCode,
    weight,
    locationShortCode,
    cancellationToken);
```

```csharp
public Task UpdateSpoolAsync(
    string shortCode,
    double? weight,
    string? locationShortCode,
    CancellationToken cancellationToken = default)
```

Use named arguments when they improve clarity, especially for:

- `bool` arguments
- `enum` arguments
- `null` arguments
- multiple arguments of the same type
- arguments whose meaning is not obvious at the call site

```csharp
await UpdateSpoolAsync(
    shortCode,
    weight: null,
    locationShortCode);
```

## Lambda Expressions

Keep short lambdas on one line.

```csharp
var names = spools.Select(spool => spool.Name);
```

When the lambda body is long, break after `=>`.

```csharp
var locations =
    spools.Where(
        spool =>
            spool.LocationId == locationId &&
            spool.Weight > 0);
```

Use block lambdas when the body contains multiple statements.

```csharp
var names =
    spools.Select(
        spool =>
        {
            var name = spool.Name.Trim();

            return name.ToUpperInvariant();
        });
```

## If Statements and Guard Clauses

Prefer guard clauses and early return/throw.

```csharp
if (_initialized)
{
    return;
}
```

Always use braces for `if` statements, even for single-line bodies.

Do not write single-line `if` statements.

```csharp
// Avoid
if (_initialized) return;
```

Prefer shallow nesting.

## Null Checks

Use pattern matching for normal null checks.

```csharp
if (location is null)
{
    return;
}
```

Do not use `== null`.

Use `ArgumentNullException.ThrowIfNull` for argument validation.

```csharp
ArgumentNullException.ThrowIfNull(logger);
```

Use nullable return types when “not found” is a normal outcome.

```csharp
Task<Location?> FindLocationAsync(string shortCode);
```

Use exceptions when failure indicates a contract violation or invalid state.

## Using Statements

Prefer using declarations.

```csharp
using var stream = File.OpenRead(path);
```

Use `await using var` for async disposable objects.

```csharp
await using var dbContext =
    await _dbContextFactory.CreateDbContextAsync();
```

Use `using` blocks only when a narrower scope is intentionally needed.

## Async and CancellationToken

Use `Async` suffix for asynchronous methods.

Library public APIs should generally accept a `CancellationToken` when the operation can be long-running or I/O-bound.

`CancellationToken` should:

- be the last parameter
- be named `cancellationToken`
- have `default` when appropriate
- be passed to lower-level APIs where possible

```csharp
public Task InitializeAsync(
    CancellationToken cancellationToken = default)
```

Application-layer code such as MAUI ViewModels does not need to require `CancellationToken` everywhere.

## Fields and Properties

Use `_camelCase` for private fields.

```csharp
private readonly ILogger<SpoolService> _logger;
```

Use `readonly` for dependencies and fields that are not reassigned.

Use auto-properties for simple properties.

Use `required` and `init` where they clearly improve correctness, especially for DTOs and options.

```csharp
public required string Name { get; init; }
```

Do not force `required` on EF Core entities when it makes EF usage awkward. For EF entities, this is acceptable:

```csharp
public string Name { get; set; } = string.Empty;
```

Use `const` for compile-time constants and `static readonly` for runtime-created values.

```csharp
private const int DefaultTimeoutSeconds = 30;

private static readonly Uri BaseUri =
    new("https://3dfilamentprofiles.com/");
```

## File Structure

Use one primary type per file.

Use file-scoped namespaces.

```csharp
namespace FilamentManager.Automation.Services;
```

Place `using` directives before the namespace.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FilamentManager.Automation.Services;
```

Keep `System` usings first and sort usings alphabetically. Do not separate using groups with blank lines.

Match namespaces to folder structure.

Do not create an `Interfaces` folder only for interface types.

Place interfaces next to their implementation when the implementation exists in the same project.

```text
Services/
  ISpoolService.cs
  SpoolService.cs
```

If the implementation is in another project, place the interface in `Abstractions`.

```text
Abstractions/
  IWebSession.cs
```

Use nested enums for private implementation details. Use independent files for public or shared enum concepts.

## Member Order

Prefer step-down order.

General order:

1. constants
2. static fields
3. instance fields
4. constructor
5. properties
6. main operation
7. helper operations used by the main operation
8. utility methods
9. dispose methods

Within the same level, prefer reading order over alphabetical order.

Place private helper methods near the public method that uses them when that improves readability.

## Comments

Write comments to explain why, not what.

Avoid comments that simply repeat the code.

```csharp
// 3dfilamentprofiles.com embeds the required data in script tags,
// so the client extracts JSON from the page instead of calling an API.
```

XML comments are not required.

Use XML comments only when they provide real value for public APIs.

Do not keep commented-out code.

TODO comments are allowed.

```csharp
// TODO: Replace this with SDK-based barcode scanning.
```

## Exceptions

Use existing exception types first.

Do not create custom exception types unless there is a clear need.

Do not use `Result<T>` by default.

Use exceptions for abnormal states or contract violations.

Use nullable return values when absence is a normal result.

Common choices:

- `ArgumentNullException` for null argument validation
- `ArgumentException` for invalid argument values
- `InvalidOperationException` for invalid object state
- `NotSupportedException` for unsupported operations

## Logging

Use `ILogger<T>`.

Use structured logging.

```csharp
_logger.LogInformation(
    "Updating spool {ShortCode}.",
    shortCode);
```

Do not use interpolated strings in log messages.

```csharp
// Avoid
_logger.LogInformation(
    $"Updating spool {shortCode}.");
```

Write log messages as natural sentences when that improves readability.

Pass exceptions as the exception argument.

```csharp
_logger.LogError(
    exception,
    "Failed to update spool {ShortCode}.",
    shortCode);
```

Do not use `EventId` unless a clear need appears.

Log processing starts, completions, important state changes, and abnormal situations. Avoid overly noisy logs in small helper methods.

## Strings

Use string interpolation for normal string composition.

```csharp
var name = $"{brandName} {materialType} {color}";
```

Do not use `string.Format` for ordinary formatting.

Use `StringBuilder` only when repeated concatenation or performance concerns justify it.

Use `string.IsNullOrEmpty` or `string.IsNullOrWhiteSpace` for empty string checks.

Specify `StringComparison` for string comparisons where appropriate.

```csharp
if (string.Equals(
        shortCode,
        input,
        StringComparison.OrdinalIgnoreCase))
{
}
```

## Switch and Pattern Matching

Use switch expressions for simple value transformations.

```csharp
string materialName =
    materialType switch
    {
        MaterialType.Pla => "PLA",
        MaterialType.Petg => "PETG",
        MaterialType.Tpu => "TPU",
        _ => "Unknown",
    };
```

Use switch statements when each case performs multiple operations.

Use pattern matching when it improves readability.

```csharp
if (location is { RemoteId: > 0 })
{
}
```

Avoid overly complex patterns that make the condition harder to read.

## Records

Use records when the type represents a value.

Records may be considered for:

- DTOs
- options
- value objects
- immutable data

Do not use records for:

- services
- EF Core entities
- ViewModels
- objects with significant mutable state or behavior

Record usage is optional, not mandatory.

## XAML

Use 4 spaces for XAML indentation.

Attributes may stay on one line when there are two or fewer attributes.

```xml
<Label Text="Location" />
```

Use one attribute per line when there are three or more attributes.

```xml
<Button
    Text="Save"
    Command="{Binding SaveCommand}"
    IsEnabled="{Binding CanSave}" />
```

Do not vertically align attributes.

Prefer a consistent attribute order:

1. `x:Name`
2. attached properties such as `Grid.Row` and `Grid.Column`
3. primary content such as `Text`, `Title`, `Source`, `Placeholder`, `ItemsSource`
4. bindings such as `Command`, `IsVisible`, `IsEnabled`, `SelectedItem`
5. layout such as `WidthRequest`, `HeightRequest`, `HorizontalOptions`, `VerticalOptions`, `Margin`, `Padding`
6. appearance such as `FontSize`, `TextColor`, `BackgroundColor`

Use `x:DataType` where practical to enable compiled bindings.

## Testing

Use xUnit.

Use AwesomeAssertions for assertions.

Use Arrange / Act / Assert.

Write AAA comments for complex tests.

AAA comments may be omitted for very simple tests.

Test class names should use:

```text
<ClassName>Tests
```

Examples:

```text
SpoolServiceTests
FilamentProfileClientTests
```

Test method names should use:

```text
<MethodName>_When<Condition>_<ExpectedResult>
```

Examples:

```text
GetLocationNameAsync_WhenLocationExists_ReturnsName
GetLocationNameAsync_WhenLocationDoesNotExist_ReturnsNull
InitializeAsync_WhenAlreadyInitialized_DoesNothing
UpdateSpoolAsync_WhenLocationIsUnknown_ThrowsInvalidOperationException
```

Do not use vague names such as:

```text
WorksCorrectly
ReturnsSuccessfully
Test1
```

Do not limit tests to one assertion when multiple assertions describe the same behavior.

Use meaningful test data instead of generic values such as `Test`.
