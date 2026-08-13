/// <summary>
/// Strongly-typed capture of all parsed sqlformat command-line values.
/// System.CommandLine 2.0 has no automatic model binding, so this is populated once
/// from a <see cref="System.CommandLine.ParseResult"/> and passed around as a single unit.
/// </summary>
internal sealed record CliArguments(
    string Sql,
    string File,
    string Output,
    bool Diagnostics,
    int? IndentSize,
    bool UseSpaces,
    bool UseTabs,
    int? MaxLineLength,
    string KeywordCasing,
    string BracketSpacing,
    string ConfigFile
);
