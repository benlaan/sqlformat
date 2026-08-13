# SQL Formatter Options Guide

The SQL Formatter now supports comprehensive formatting options!

## Configuration Methods

### 1. Config File (.sqlformat.json)

Create a `.sqlformat.json` file in your project root:

```json
{
  "indentSize": 2,
  "useSpaces": true,
  "maxLineLength": 120,
  "keywordCasing": "Lower",
  "bracketSpacing": "NoSpaces",
  "blankLinesBetweenClauses": 1,
  "maxInlineSelectColumns": 1,
  "maxInlineInsertColumns": 4
}
```

The formatter searches for config files in this order:
1. Current directory
2. Parent directories (up to root)
3. User home directory

### 2. Console App Command-Line Arguments

The CLI is built on [`System.CommandLine`](https://www.nuget.org/packages/System.CommandLine) and uses
POSIX-style `--long`/`-short` options.

```bash
# Lowercase keywords with 2-space indent
sqlformat --file input.sql --keyword-casing lower --indent-size 2

# Use tabs and 120-char line length
sqlformat --sql "SELECT * FROM Users" --use-tabs --max-line-length 120

# Specify config file explicitly
sqlformat --file input.sql --config-file custom-format.json

# Pipe input with options
cat query.sql | sqlformat --keyword-casing pascal --indent-size 4

# Built-in help
sqlformat --help
```

Available command-line options:
- `--sql`, `-s <text>` - SQL text to format
- `--file`, `-f <path>` - Path to a SQL file to format
- `--output`, `-o <path>` - Path to write the formatted output (defaults to stdout)
- `--diagnostics`, `-d` - Print elapsed formatting time
- `--indent-size <number>` - Spaces/tabs per indent level (default: 4)
- `--use-spaces` - Use spaces for indentation (default: true)
- `--use-tabs` - Use tabs for indentation
- `--max-line-length <number>` - Max line length before wrapping (default: 80)
- `--keyword-casing <style>` - Upper, Lower, or Pascal (default: Upper)
- `--bracket-spacing <style>` - NoSpaces or WithSpaces (default: NoSpaces)
- `--config-file <path>` - Path to .sqlformat.json config file

### 3. Blazor Web UI

The web interface includes a collapsible **Formatting Options** panel with:

- **Indent Size** - Numeric input (0-16)
- **Use Spaces** - Toggle switch
- **Max Line Length** - Numeric input (20-1000)
- **Keyword Casing** - Dropdown: UPPERCASE, lowercase, PascalCase
- **Bracket Spacing** - Dropdown: (value) or ( value )
- **Blank Lines Between Clauses** - Numeric input (0-5)
- **Max Inline SELECT Columns** - Numeric input
- **Max Inline INSERT Columns** - Numeric input

Options are saved to browser LocalStorage automatically when you click "Save Options" and persist across sessions.

### 4. Programmatic API

```csharp
using Laan.Sql.Formatter;

// Using default options
var engine = new FormattingEngine();
var formatted = engine.Execute(sql);

// Using custom options
var options = new FormattingOptions
{
    IndentSize = 2,
    UseSpaces = true,
    MaxLineLength = 120,
    KeywordCasing = KeywordCasing.Lower,
    BracketSpacing = BracketSpacing.NoSpaces,
    BlankLinesBetweenClauses = 1,
    MaxInlineSelectColumns = 1,
    MaxInlineInsertColumns = 4
};
var engine = new FormattingEngine(options);
var formatted = engine.Execute(sql);

// Load from config file
var engine = FormattingEngine.CreateWithConfigFile(".sqlformat.json");
var formatted = engine.Execute(sql);

// Auto-discover config in directory hierarchy
var options = FormattingOptionsLoader.TryLoadFromHierarchy();
var engine = new FormattingEngine(options);
```

## Available Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `indentSize` | int | 4 | Number of spaces or tabs per indent level |
| `useSpaces` | bool | true | Use spaces (true) or tabs (false) for indentation |
| `maxLineLength` | int | 80 | Maximum line length before wrapping |
| `keywordCasing` | enum | Upper | SQL keyword casing: Upper, Lower, or Pascal |
| `bracketSpacing` | enum | NoSpaces | Bracket spacing: NoSpaces `(value)` or WithSpaces `( value )` |
| `blankLinesBetweenClauses` | int | 1 | Number of blank lines between major SQL clauses |
| `maxInlineSelectColumns` | int | 1 | Maximum SELECT columns to display inline before wrapping |
| `maxInlineInsertColumns` | int | 4 | Maximum INSERT columns to display inline before wrapping |

## Examples

### Compact Style
```json
{
  "indentSize": 2,
  "maxLineLength": 120,
  "keywordCasing": "Lower",
  "maxInlineSelectColumns": 3
}
```

### Traditional Style
```json
{
  "indentSize": 4,
  "maxLineLength": 80,
  "keywordCasing": "Upper",
  "bracketSpacing": "NoSpaces",
  "blankLinesBetweenClauses": 2
}
```

### Modern Style
```json
{
  "indentSize": 2,
  "useSpaces": true,
  "maxLineLength": 120,
  "keywordCasing": "Pascal",
  "blankLinesBetweenClauses": 1
}
```
