param(
    [string]$config = "Release",
    [string]$build = ""
)

.\.nuget\nuget.exe restore

& msbuild.exe .\Laan.Sql.Tools.sln /p:configuration=$config /p:Version=$version /p:BUILD_NUMBER=$build /v:minimal

$tmp = [System.IO.Path]::GetTempFileName()
cp "$pwd\Laan.Sql.Parser\bin\$config\Laan.Sql.Parser.dll" $tmp
$version = [System.Reflection.Assembly]::LoadFile($tmp).GetName().Version.ToString()

if(!(Test-Path "dist")) {

    New-Item -ItemType Directory "dist" | Out-Null
}

cp Laan.Sql.Tools.Installer\bin\$config\Laan.Sql.Tools.Installer.msi "dist\Laan.Sql.Tools.Installer.$version.msi" -Force
