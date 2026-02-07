# Build Scripts

## build.ps1

This script builds the SQL Formatter solution and optionally creates a packaged VSCode extension with bundled sqlformat binaries for all platforms.

### Usage

```powershell
# Basic build (Debug configuration)
.\Scripts\build.ps1

# Release build with all binaries
.\Scripts\build.ps1 -config Release

# Release build and package the VSCode extension
.\Scripts\build.ps1 -config Release -PackageExtension

# Skip .NET solution build (useful when only updating extension packaging)
.\Scripts\build.ps1 -SkipDotnetBuild -PackageExtension
```

### Parameters

- **`-config`**: Build configuration (`Debug` or `Release`). Default: `Release`
- **`-PackageExtension`**: If specified, builds and packages the VSCode extension as a `.vsix` file
- **`-SkipDotnetBuild`**: If specified, skips building the main .NET solution

### What it does

1. **Reads version** from `Directory.Build.props` (Major.Minor.Build)
2. **Builds the solution** (unless `-SkipDotnetBuild` is specified)
3. **Publishes self-contained binaries** for multiple platforms:
   - Windows x64 (`win-x64`)
   - macOS Intel (`osx-x64`)
   - macOS ARM (`osx-arm64`)
   - Linux x64 (`linux-x64`)
4. **Copies binaries** to `Laan.SqlFormatter.VsCode\bin\{platform}\sqlformat[.exe]`
5. **Updates** `package.json` version to match .NET version
6. **Optionally packages** the VSCode extension as a `.vsix` file

### Output

- Binaries: `Laan.SqlFormatter.VsCode\bin\{platform}\`
- VSCode extension package: `Laan.SqlFormatter.VsCode\sqlformat-{version}.vsix`

### Requirements

- .NET SDK 9.0 or later
- Node.js and npm (for packaging the extension)
- PowerShell 5.1 or later / PowerShell Core

### Notes

The bundled binaries are self-contained single-file executables, meaning they don't require .NET to be installed on the target machine. The extension will automatically detect the platform and use the appropriate binary.
