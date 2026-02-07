# SQL Format Extension

A Visual Studio Code extension that provides SQL formatting and code snippets for Microsoft SQL Server. The extension uses a custom SQL formatter built with the Laan.Sql.Formatter library and includes over 130 helpful SQL snippets.

## Features

### SQL Formatting
- **Format Document** - Use VS Code's built-in format document command (Shift+Alt+F / Shift+Option+F)
- **Format Selection** - Format only selected SQL text with `Ctrl+Alt+F` (Windows/Linux) or `Cmd+Alt+F` (Mac)
- **Intelligent formatting** - Preserves intent while improving readability with proper indentation and spacing
- **Error reporting** - Clear error messages displayed in dedicated output channel
- **No external dependencies** - Self-contained sqlformat binaries bundled for all platforms
- **Cross-platform** - Windows, macOS (Intel & ARM), and Linux support

### SQL Snippets
Over 130 code snippets for common SQL Server patterns:
- **Transactions:** `br`, `bc`, `bt`, `ct` - Transaction blocks and control
- **SELECT queries:** `ssf`, `sfw`, `sct` - Common SELECT patterns  
- **DML:** `iv`, `is`, `ufw`, `df`, `dfw` - INSERT, UPDATE, DELETE operations
- **Joins:** `j`, `lj`, `rj`, `fj`, `cj` - All join types
- **CTEs:** `cte`, `rcte` - Common Table Expressions
- **Variables:** `di`, `dv`, `du`, `dt` - DECLARE statements for all types
- **Control flow:** `ib`, `ibe`, `tc`, `wl` - IF, TRY/CATCH, WHILE blocks
- **Functions:** `da`, `ddf`, `coal`, `cast`, `conv` - Common functions
- **DDL:** `crt`, `crp`, `crf`, `crv` - CREATE statements
- **Advanced:** `mrg`, `pv`, `rn`, `ap` - MERGE, PIVOT, window functions

Type a prefix and press Tab to expand. See the full snippet list in the extension's `snippets/sql.json` file.

## About the SQL Formatter

The bundled `sqlformat` tool is built on the [Laan.Sql.Formatter](https://github.com/benlaan/sqlformat) library, a custom SQL Server T-SQL formatter designed to produce readable, consistently formatted code. The formatter:

- Understands T-SQL syntax including CTEs, subqueries, and complex expressions
- Preserves semantic meaning while improving structure
- Handles stored procedures, functions, views, and ad-hoc queries
- Provides intelligent indentation and alignment
- Built with .NET and distributed as self-contained executables

## Requirements

**None!** The extension includes self-contained sqlformat binaries for all supported platforms. No additional installation required.

> The extension automatically detects your platform and uses the appropriate binary. If the bundled binary is not found, it will fall back to using `sqlformat` from your system PATH.

## Usage

### Formatting
1. Open a SQL file or select SQL text
2. Use one of these methods:
   - Press `Shift+Alt+F` (Windows/Linux) or `Shift+Option+F` (Mac) to format the document
   - Press `Ctrl+Alt+F` (Windows/Linux) or `Cmd+Alt+F` (Mac) to format selection
   - Right-click → "Format Document" or "Format Selection"
   - Command Palette → "Format Document" or "SQL Format: Format Selection"

### Snippets
1. Type a snippet prefix (e.g., `ssf`, `cte`, `tc`)
2. Press Tab to expand
3. Use Tab to navigate between placeholders

### Setting as Default Formatter
To make this the default formatter for SQL files:
1. Open a SQL file
2. Right-click → "Format Document With..." → "Configure Default Formatter..."
3. Select "SQL Format"

Or add to your VS Code settings:
```json
"[sql]": {
  "editor.defaultFormatter": "laan.sqlformat"
}
```

## Extension Settings

This extension contributes the following settings for customizing SQL formatting:

### Formatting Options

Configure these settings in VS Code User or Workspace settings:

- **`sqlformat.indentSize`** (number, default: `4`) - Number of spaces or tabs per indent level (0-16)
- **`sqlformat.useSpaces`** (boolean, default: `true`) - Use spaces for indentation (true) or tabs (false)
- **`sqlformat.maxLineLength`** (number, default: `80`) - Maximum line length before wrapping (20-1000)
- **`sqlformat.keywordCasing`** (string, default: `"Upper"`) - Keyword casing style:
  - `"Upper"` - UPPERCASE keywords
  - `"Lower"` - lowercase keywords
  - `"Pascal"` - PascalCase keywords
- **`sqlformat.bracketSpacing`** (string, default: `"NoSpaces"`) - Bracket spacing style:
  - `"NoSpaces"` - No spaces: `(value)`
  - `"WithSpaces"` - With spaces: `( value )`
- **`sqlformat.blankLinesBetweenClauses`** (number, default: `1`) - Number of blank lines between major SQL clauses (0-5)
- **`sqlformat.maxInlineSelectColumns`** (number, default: `1`) - Maximum SELECT columns to display inline before wrapping
- **`sqlformat.maxInlineInsertColumns`** (number, default: `4`) - Maximum INSERT columns to display inline before wrapping
- **`sqlformat.configFile`** (string, default: `""`) - Path to `.sqlformat.json` config file (leave empty to auto-discover)

### Configuration Methods

**Option 1: VS Code Settings**

Open Settings (Ctrl+, or Cmd+,) and search for "sqlformat", or add to `settings.json`:

```json
{
  "sqlformat.indentSize": 2,
  "sqlformat.useSpaces": true,
  "sqlformat.maxLineLength": 120,
  "sqlformat.keywordCasing": "Lower",
  "sqlformat.bracketSpacing": "NoSpaces"
}
```

**Option 2: .sqlformat.json File**

Create a `.sqlformat.json` file in your workspace root:

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

The extension will automatically search for `.sqlformat.json` in:
1. Current workspace root
2. Parent directories
3. User home directory

**Configuration Priority:**
1. Explicit `sqlformat.configFile` setting (if specified)
2. Auto-discovered `.sqlformat.json` file
3. VS Code settings
4. Default values

### Example Configurations

**Compact Style:**
```json
{
  "sqlformat.indentSize": 2,
  "sqlformat.maxLineLength": 120,
  "sqlformat.keywordCasing": "Lower",
  "sqlformat.maxInlineSelectColumns": 3
}
```

**Traditional Style:**
```json
{
  "sqlformat.indentSize": 4,
  "sqlformat.maxLineLength": 80,
  "sqlformat.keywordCasing": "Upper",
  "sqlformat.blankLinesBetweenClauses": 2
}
```

## License

MIT
