param(
    [string]$config = "debug",
    [switch]$restore
)

dotnet build .\Laan.Sql.Tools.sln -c $config -v minimal --interactive $(if ($restore.IsPresent) { "" } else { "--no-restore" } )