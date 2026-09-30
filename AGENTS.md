# AGENTS.md — sqlformat

A T-SQL formatter with a parser library, CLI tool, Blazor WebAssembly UI, SSMS add-in, NHibernate appender, VSCode extension, and WiX installer.

## Architecture

```
Laan.Sql.Parser/         — T-SQL tokenizer + parser (library)
Laan.Sql.Formatter/      — Formatting engine, depends on parser (library)
Laan.Sql.Formatter.Console/ — CLI: assembly name = sqlformat.exe
Laan.Sql.Formatter.Test/ — NUnit tests
Laan.Sql.Parser.Test/    — NUnit tests
Laan.Sql.Formatter.Web.Blazor/ — Blazor WASM UI
Laan.SqlFormatter.VsCode/     — VSCode extension (TypeScript, esbuild)
Laan.AddIns.Ssms.VsExtension/ — SSMS extension
Laan.Sql.Tools.Installer/     — WiX installer (.wixproj)
Laan.NHibernate.Appender/     — NHibernate log appender
```

## Commands

| Action | Command |
|--------|---------|
| Build all | `dotnet build Laan.Sql.Tools.sln -c Release` |
| Run all tests | `dotnet test Laan.Sql.Tools.sln` or `./test.ps1` |
| Test (pre-built) | `dotnet test --no-restore --no-build` |
| Test one project | `dotnet test Laan.Sql.Formatter.Test/` |
| Full release + .vsix | `./Scripts/build.ps1 -PackageExtension` |
| Pack NuGet | `./pack.ps1` |

## Key conventions

- **Test framework**: NUnit (not xUnit). Tests extend `BaseFormattingTest` and use `Compare(actual, expectedLines)` — an **array of strings**, not a single string.
- **TreatWarningsAsErrors** is on globally. Warnings fail the build.
- **Multi-target**: libraries target `net472;net481;net9.0;net10.0`. Console targets only `net10.0`. Blazor WASM targets `net10.0`.
- **Assembly info** is auto-generated into `/Generated/`; that dir is in `.gitignore`.
- **Config**: `.sqlformat.json` is auto-discovered (current dir → parent dirs → home). CLI also accepts `-ConfigFile path`.
- **Source gen**: `FormattingOptionsJsonContext` uses `System.Text.Json` source generators on .NET 6+.

## Project-specific

- **ParserFactory.cs** maps statement-leading keywords (SELECT, INSERT, etc.) to parser classes via a switch statement.
- **FormattingEngine** is the main library entrypoint: `new FormattingEngine(options).Execute(sql)`.
- **CLI usage**: `sqlformat --sql "SELECT * FROM T" --file input.sql --output out.sql`. Also supports stdin piping. Uses POSIX-style `--long`/`-short` flags via `System.CommandLine` (no legacy PascalCase aliases).
- **VSCode extension**: lives in `Laan.SqlFormatter.VsCode/`. Built with `esbuild`, not tsc. Run `npm run compile` or `npm run watch` inside that directory. Register/ship via `vsce package` (done by `build.ps1`).
- **build.ps1** is the canonical release pipeline: builds solution, publishes self-contained binaries for win-x64/osx-x64/osx-arm64/linux-x64, updates version in package.json, then optionally packages the .vsix.
- **SSMS extension** uses `.vsix` as well — not the same as the VSCode one.

## Testing quirks

- Tests compare formatted output line-by-line against expected string arrays. The `Compare` helper normalizes line endings first.
- Test classes are organized by statement type (e.g. `TestSelectStatementFormatting`, `TestInsertStatementFormatting`).
- There are no integration tests requiring a database.
