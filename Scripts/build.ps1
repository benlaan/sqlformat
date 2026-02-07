param(
    [string]$config = "Release",
    [string]$build = "",
    [switch]$PackageExtension = $false,
    [switch]$SkipDotnetBuild = $false,
    [switch]$Install = $false
)

$ErrorActionPreference = "Stop"

# Get version from Directory.Build.props
$buildPropsPath = Join-Path $PSScriptRoot "..\Directory.Build.props"
[xml]$buildProps = Get-Content $buildPropsPath
$major = $buildProps.Project.PropertyGroup[0].Major
$minor = $buildProps.Project.PropertyGroup[0].Minor
$build = $buildProps.Project.PropertyGroup[0].Build
$version = "$major.$minor.$build"

Write-Host "Building SQL Formatter Tools v$version" -ForegroundColor Cyan
Write-Host "Configuration: $config" -ForegroundColor Cyan
Write-Host ""

# Build main solution if not skipped
if (-not $SkipDotnetBuild) {
    Write-Host "Building main solution..." -ForegroundColor Yellow
    dotnet build .\Laan.Sql.Tools.sln -c $config -v minimal --interactive
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Solution build failed"
        exit $LASTEXITCODE
    }
    
    # Build the formatter library for net10.0 specifically (needed for publishing)
    Write-Host "Building Laan.Sql.Formatter for net10.0..." -ForegroundColor Yellow
    $solutionDir = Join-Path $PSScriptRoot ".." | Resolve-Path
    dotnet build .\Laan.Sql.Formatter\Laan.Sql.Formatter.csproj -c $config -f net10.0 -v minimal /p:SolutionDir="$solutionDir\"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Formatter build failed"
        exit $LASTEXITCODE
    }
    Write-Host ""
}

# Define target platforms
$platforms = @(
    @{ RID = "win-x64"; Ext = ".exe" },
    @{ RID = "osx-x64"; Ext = "" },
    @{ RID = "osx-arm64"; Ext = "" },
    @{ RID = "linux-x64"; Ext = "" }
)

$consoleProject = ".\Laan.Sql.Formatter.Console\Laan.Sql.Formatter.Console.csproj"
$vscodeExtPath = ".\Laan.SqlFormatter.VsCode"
$binPath = Join-Path $vscodeExtPath "bin"

# Clean and create bin directory structure
Write-Host "Preparing VSCode extension binary directory..." -ForegroundColor Yellow
if (Test-Path $binPath) {
    Remove-Item $binPath -Recurse -Force
}
New-Item -ItemType Directory -Path $binPath -Force | Out-Null

# Publish for each platform
Write-Host "Publishing self-contained binaries for all platforms..." -ForegroundColor Yellow
foreach ($platform in $platforms) {
    $rid = $platform.RID
    $ext = $platform.Ext

    Write-Host "  Publishing for $rid..." -ForegroundColor Gray

    $publishArgs = @(
        "publish",
        $consoleProject,
        "-c", $config,
        "-f", "net10.0",
        "-r", $rid,
        "--self-contained", "true",
        "/p:PublishSingleFile=true",
        "/p:IncludeNativeLibrariesForSelfExtract=true",
        "/p:EnableCompressionInSingleFile=true",
        "/p:SolutionDir=$PSScriptRoot\..\",
        "-o", ".\publish\$rid",
        "-v", "minimal"
    )

    Write-Host "    Running: dotnet $($publishArgs -join ' ')" -ForegroundColor DarkGray

    & dotnet @publishArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Failed to publish for $rid"
        exit $LASTEXITCODE
    }

    # Copy binary to VSCode extension bin folder
    $targetDir = Join-Path $binPath $rid
    New-Item -ItemType Directory -Path $targetDir -Force | Out-Null

    $sourceBinary = ".\publish\$rid\sqlformat$ext"
    $targetBinary = Join-Path $targetDir "sqlformat$ext"

    Copy-Item $sourceBinary $targetBinary -Force

    # Set executable permissions for Unix-like systems (this metadata will be preserved in the .vsix)
    if ($ext -eq "") {
        # We'll need to ensure the VSCode extension packaging preserves these permissions
        Write-Host "    Binary copied to: $targetBinary" -ForegroundColor DarkGray
    }
}

Write-Host ""
Write-Host "Cleaning up publish artifacts..." -ForegroundColor Yellow
Remove-Item ".\publish" -Recurse -Force -ErrorAction SilentlyContinue

# Update package.json version
$packageJsonPath = Join-Path $vscodeExtPath "package.json"
Write-Host "Updating package.json version to $version..." -ForegroundColor Yellow

$packageJson = Get-Content $packageJsonPath -Raw | ConvertFrom-Json
$packageJson.version = $version
$packageJson | ConvertTo-Json -Depth 100 | Set-Content $packageJsonPath -Encoding UTF8

Write-Host ""

# Build and package VSCode extension
if ($PackageExtension) {
    Write-Host "Building VSCode extension..." -ForegroundColor Yellow
    Push-Location $vscodeExtPath
    try {
        # Install dependencies if needed
        if (-not (Test-Path "node_modules")) {
            Write-Host "  Installing npm dependencies..." -ForegroundColor Gray
            npm install
            if ($LASTEXITCODE -ne 0) {
                Write-Error "npm install failed"
                exit $LASTEXITCODE
            }
        }
        
        # Build the extension
        Write-Host "  Compiling TypeScript..." -ForegroundColor Gray
        npm run package
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Extension build failed"
            exit $LASTEXITCODE
        }
        
        # Package the extension
        Write-Host "  Creating .vsix package..." -ForegroundColor Gray
        npx @vscode/vsce package
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Extension packaging failed"
            exit $LASTEXITCODE
        }
        
        Write-Host ""
        Write-Host "Extension packaged successfully!" -ForegroundColor Green
        $vsixFile = Get-ChildItem "sqlformat-$version.vsix" -ErrorAction SilentlyContinue
        if ($vsixFile) {
            Write-Host "Package: $($vsixFile.FullName)" -ForegroundColor Green
        }
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Host "Skipping extension packaging (use -PackageExtension to create .vsix)" -ForegroundColor DarkGray
}

Write-Host ""
Write-Host "Build completed successfully!" -ForegroundColor Green
Write-Host "Binaries are in: $binPath" -ForegroundColor Green

if ($Install) {
    Write-Host "Installing VSCode extension..." -ForegroundColor Yellow
    $vsixFile = Get-ChildItem "$vscodeExtPath\sqlformat-$version.vsix" -ErrorAction SilentlyContinue
    if ($vsixFile) {
        # Uninstall existing extension first to ensure clean install
        Write-Host "  Uninstalling existing extension (if present)..." -ForegroundColor Gray
        code --uninstall-extension benlaan.sqlformat 2>$null
        # Ignore exit code - extension might not be installed
        
        Write-Host "  Installing $($vsixFile.FullName)..." -ForegroundColor Gray
        code --install-extension $vsixFile.FullName --force
        if ($LASTEXITCODE -ne 0) {
            Write-Error "Extension installation failed"
            exit $LASTEXITCODE
        }
        Write-Host "Extension installed successfully!" -ForegroundColor Green
    }
    else {
        Write-Error "VSIX package not found. Please build the extension first with -PackageExtension."
        exit 1
    }
}