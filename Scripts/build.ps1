param(
    [string]$config = "debug",
    [string]$build = ""
)

dotnet build .\Laan.Sql.Tools.sln -c $config -v minimal --interactive
