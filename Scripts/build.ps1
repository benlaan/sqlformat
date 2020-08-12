param(
    [string]$config = "debug",
    [string]$build = ""
)

& msbuild.exe .\Laan.Sql.Tools.sln /p:configuration=$config /v:minimal
