# Laan Sql Tools (sqlformat)

A hand-written T-SQL tokenizer/parser/formatter in C#, packaged as a NuGet library, a CLI (`sqlformat`), a VS Code extension, an SSMS add-in, and a Blazor web UI.

## Build / Test / Lint

- Full solution build: `dotnet build .\Laan.Sql.Tools.sln -c Release`
- Full test run: `dotnet test --no-restore --no-build` (from repo root; requires a prior `dotnet build`)
- Run a single test project: `dotnet test .\Laan.Sql.Formatter.Test\Laan.Sql.Formatter.Test.csproj --no-build -c Debug`
- Run a single test/fixture (NUnit): add `--filter "FullyQualifiedName~TestSelectStatementFormatting"` or `--filter "Name=SomeTestMethod"` to the `dotnet test` command above.
- Test framework is **NUnit 3** (not xUnit/MSTest) — new tests must use `[TestFixture]`/`[Test]` and `Assert.AreEqual`.
- `Scripts\test.ps1` runs every `*Test*.csproj` under the repo, produces TRX files + a `TestResults\summary.csv`.
- `Scripts\build.ps1` (or `build.cmd`) does a full release build, publishes self-contained `sqlformat` binaries for win-x64/osx-x64/osx-arm64/linux-x64 into `Laan.SqlFormatter.VsCode\bin\`, and optionally packages the VS Code extension (`-PackageExtension`, requires npm) or installs it (`-Install`).
- `pack.ps1` builds NuGet packages via `dotnet pack` into `.\Packages`.
- Target frameworks are multi-targeted: `net472;net481;net9.0;net10.0`. `TreatWarningsAsErrors` is enabled solution-wide (`Directory.Build.props`), so warnings fail the build.
- Version is centrally defined in `Directory.Build.props` (`Major.Minor.Build`) and consumed by `AssemblyInfo` generation and the VS Code extension's `package.json`.

## Architecture

Three-layer pipeline, each layer in its own project:

1. **`Laan.Sql.Parser`** — tokenizes and parses raw SQL text into an object model.
   - `Tokenizer/` turns SQL text into `Token`s.
   - `Parsers/` contains one parser per statement type (e.g. `SelectStatementParser`, `InsertStatementParser`), all subclassing `StatementParser<T>` (in `StatementParser.cs`).
   - `ParserFactory` is a static dictionary/switch that maps the leading SQL keyword (see `Constants`) to the matching parser class. **Adding a new statement type requires updating both** the `_parsers` dictionary and the `switch` in `ParserFactory.GetParser`.
   - `Entities/` holds the parsed statement/expression object model (`Statement`, `IStatement`, etc.); `Expressions/` holds expression-level parsing (WHERE clauses, functions, CASE, etc.).

2. **`Laan.Sql.Formatter`** — walks the parsed statement tree and re-emits formatted SQL.
   - `StatementFormatters/` mirrors `Parsers/` 1:1 — one formatter per statement type, subclassing `StatementFormatter<T>` (in `StatementFormatter.cs`).
   - `Factories/StatementFormatterFactory.cs` is a `switch` over statement type that dispatches to the matching formatter — **every new statement type needs an entry here too** (mirroring the parser factory), or a `FormatterNotImplementedException` is thrown.
   - `ExpressionFormatters/` + `Factories/ExpressionFormatterFactory.cs` follow the same pattern for expression nodes.
   - `FormattingEngine.Execute(sql)` is the single public entry point: runs `ParserFactory.Execute` then formats each resulting `IStatement` via the factory.
   - `FormattingOptions` (+ `FormattingOptionsLoader`) controls indent size/char, max line length, keyword casing, bracket spacing, blank lines between clauses, and inline column limits. Options are loaded from `.sqlformat.json`, searched from the current directory up through parent directories to the user home directory (`FormattingOptionsLoader.TryLoadFromHierarchy`). See `OPTIONS.md` for the full schema and config precedence.

3. **Consumers** wrap `FormattingEngine`/`ParserFactory` — they should not contain formatting logic themselves:
   - `Laan.Sql.Formatter.Console` — the `sqlformat` CLI (`Program.cs`/`Argument.cs`); reads SQL from `-Sql`, `-File`, or stdin, resolves `FormattingOptions` from CLI args > explicit `-ConfigFile` > directory-hierarchy search, then calls `FormattingEngine.Execute`.
   - `Laan.SqlFormatter.VsCode` — a VS Code extension (TypeScript) that shells out to the published `sqlformat` binary. It has its own `.github/copilot-instructions.md` — **check it when working in that subfolder**, as it documents the extension-specific structure and workflow.
   - `Laan.AddIns.Ssms.VsExtension` — SSMS add-in.
   - `Laan.Sql.Formatter.Web.Blazor` — Blazor web UI wrapping the same engine, with an options panel persisted to LocalStorage.
   - `Laan.NHibernate.Appender` — a log4net/NHibernate appender that uses `ParserFactory.TrimBatchMetadata` to strip NHibernate batch-command prefixes before parsing logged SQL.

## Key Conventions

- **Adding a new SQL statement type is a three-file change**: a new `Entities` class implementing `IStatement`, a new `Parsers/XxxStatementParser` registered in `ParserFactory`, and a new `StatementFormatters/XxxStatementFormatter` registered in `StatementFormatterFactory`. Corresponding tests live in `Laan.Sql.Formatter.Test/TestXxxStatementFormatting.cs` (formatter round-trip tests) and/or `Laan.Sql.Parser.Test` (parser-only tests).
- Formatter tests use `BaseFormattingTest.Compare(actual, expected)`, which normalizes line endings and renders a side-by-side diff (using `·`/`¶`/`§` markers for spaces/CR/LF) on failure — prefer this helper over raw `Assert.AreEqual` for formatting output.
- Formatting code always goes through `Keyword(...)` (casing), `Indent`/`IndentScope`/`NewLine` (indentation), and `Append`/`IndentAppend` helpers on `BaseFormatter` rather than raw string concatenation, so options like `KeywordCasing`, `IndentSize`, and `MaxLineLength` are respected consistently.
- `FormattingOptions` is the single source of truth for formatting behavior — don't add new formatting toggles as ad-hoc parameters; add them to `FormattingOptions`/`.sqlformat.schema.json` and thread them through the loader, CLI args (`Argument.cs`), and Blazor UI together.
- This is an older codebase with plenty of pre-existing legacy C# style (explicit types, verbose null checks, etc.). Existing code doesn't need to be rewritten, but **new or modified code should use modern C# constructs** where they improve clarity — e.g. `var` for local declarations instead of explicit types, pattern matching, expression-bodied members — rather than mimicking the surrounding old-fashioned style.
- Some older code uses "spacious" call-site formatting (e.g. `Method( a, b )`, `String.Format( "{0}", x )` with spaces just inside parens). **New/modified code should use the compact, typical style instead** — `Method(a, b)`, no spaces just inside parentheses — even when surrounded by older spacious-style code.
