param([string]$version)

& msbuild.exe .\Laan.Sql.Tools.sln /p:configuration=Release /p:Version=$version /v:minimal

cp Laan.Tools.Sql.Installer\bin\Release\Laan.Sql.Tools.Installer.msi ".\Laan.Sql.Tools.Installer.$version.msi"

