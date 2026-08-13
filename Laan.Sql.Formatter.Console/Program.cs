using System;
using System.CommandLine;
using System.IO;
using System.Text;

using Laan.Sql.Formatter;

internal static class Program
{
    private static int Main(string[] args)
    {
        var sqlOption = new Option<string>("--sql", "-s")
        {
            Description = "SQL text to format"
        };
        var fileOption = new Option<string>("--file", "-f")
        {
            Description = "Path to a SQL file to format"
        };
        var outputOption = new Option<string>("--output", "-o")
        {
            Description = "Path to write the formatted output (defaults to stdout)"
        };
        var diagnosticsOption = new Option<bool>("--diagnostics", "-d")
        {
            Description = "Print elapsed formatting time"
        };
        var indentSizeOption = new Option<int?>("--indent-size")
        {
            Description = "Number of spaces per indent level (default: 4)"
        };
        var useSpacesOption = new Option<bool>("--use-spaces")
        {
            Description = "Use spaces for indentation (default: true)"
        };
        var useTabsOption = new Option<bool>("--use-tabs")
        {
            Description = "Use tabs for indentation"
        };
        var maxLineLengthOption = new Option<int?>("--max-line-length")
        {
            Description = "Maximum line length before wrapping (default: 80)"
        };
        var keywordCasingOption = new Option<string>("--keyword-casing")
        {
            Description = "Keyword casing: Upper, Lower, or Pascal (default: Upper)"
        };
        var bracketSpacingOption = new Option<string>("--bracket-spacing")
        {
            Description = "Bracket spacing: NoSpaces or WithSpaces (default: NoSpaces)"
        };
        var configFileOption = new Option<string>("--config-file")
        {
            Description = "Path to .sqlformat.json config file"
        };

        var rootCommand = new RootCommand(
            "sqlformat - a T-SQL formatter. If no options are specified, searches for " +
            ".sqlformat.json in current/parent directories.\n" +
            "Usage: sqlformat --sql \"select 1\" | --file input.sql [--output out.sql] [--diagnostics]\n" +
            "   or: cat file.sql | sqlformat [--output out.sql] [--diagnostics]");

        rootCommand.Options.Add(sqlOption);
        rootCommand.Options.Add(fileOption);
        rootCommand.Options.Add(outputOption);
        rootCommand.Options.Add(diagnosticsOption);
        rootCommand.Options.Add(indentSizeOption);
        rootCommand.Options.Add(useSpacesOption);
        rootCommand.Options.Add(useTabsOption);
        rootCommand.Options.Add(maxLineLengthOption);
        rootCommand.Options.Add(keywordCasingOption);
        rootCommand.Options.Add(bracketSpacingOption);
        rootCommand.Options.Add(configFileOption);

        rootCommand.SetAction(parseResult => Run(new CliArguments(
            Sql: parseResult.GetValue(sqlOption),
            File: parseResult.GetValue(fileOption),
            Output: parseResult.GetValue(outputOption),
            Diagnostics: parseResult.GetValue(diagnosticsOption),
            IndentSize: parseResult.GetValue(indentSizeOption),
            UseSpaces: parseResult.GetValue(useSpacesOption),
            UseTabs: parseResult.GetValue(useTabsOption),
            MaxLineLength: parseResult.GetValue(maxLineLengthOption),
            KeywordCasing: parseResult.GetValue(keywordCasingOption),
            BracketSpacing: parseResult.GetValue(bracketSpacingOption),
            ConfigFile: parseResult.GetValue(configFileOption))));

        // No args and no piped input is an interactive usage error - show help and exit non-zero,
        // rather than silently attempting (and failing) to read SQL from an attached terminal.
        if (args.Length == 0 && !Console.IsInputRedirected)
        {
            rootCommand.Parse("--help").Invoke();
            return 1;
        }

        return rootCommand.Parse(args).Invoke();
    }

    private static int Run(CliArguments args)
    {
        // Load formatting options
        var options = LoadOptions(args);
        var engine = new FormattingEngine(options);

        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        try
        {
            string sqlText = null;

            if (args.Sql != null)
                sqlText = args.Sql;
            else if (args.File != null && File.Exists(args.File))
                sqlText = File.ReadAllText(args.File);
            else if (Console.IsInputRedirected)
            {
                // Read from stdin if input is redirected (piped)
                using (var reader = Console.In)
                {
                    sqlText = reader.ReadToEnd();
                }
            }

            if (sqlText == null)
            {
                Console.WriteLine("No SQL found - either supply --sql or --file arguments, or pipe SQL via stdin");
                return 2;
            }

            var formattedOutput = engine.Execute(sqlText);

            if (args.Output != null)
            {
                File.WriteAllText(args.Output, formattedOutput, Encoding.UTF8);
            }
            else
            {
                var formattedSql = formattedOutput.TrimEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
                foreach (var line in formattedSql)
                    Console.WriteLine(line);
            }

            if (args.Diagnostics)
                Console.WriteLine("\nElapsed Time: " + TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds));

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return 3;
        }
        finally
        {
            timer.Stop();
        }
    }

    private static FormattingOptions LoadOptions(CliArguments args)
    {
        FormattingOptions options;

        // Load from config file if specified, otherwise search hierarchy
        if (!string.IsNullOrEmpty(args.ConfigFile))
        {
            options = FormattingOptionsLoader.LoadFromFile(args.ConfigFile);
        }
        else
        {
            // Try to load from hierarchy (current dir -> parent dirs -> home dir)
            options = FormattingOptionsLoader.TryLoadFromHierarchy();
        }

        // Override with command-line arguments
        if (args.IndentSize.HasValue)
            options.IndentSize = args.IndentSize.Value;

        if (args.UseTabs)
            options.UseSpaces = false;
        else if (args.UseSpaces)
            options.UseSpaces = true;

        if (args.MaxLineLength.HasValue)
            options.MaxLineLength = args.MaxLineLength.Value;

        if (!string.IsNullOrEmpty(args.KeywordCasing))
        {
            if (Enum.TryParse<KeywordCasing>(args.KeywordCasing, true, out var casing))
                options.KeywordCasing = casing;
            else
                throw new ArgumentException($"Invalid KeywordCasing value: {args.KeywordCasing}. Valid values are: Upper, Lower, Pascal");
        }

        if (!string.IsNullOrEmpty(args.BracketSpacing))
        {
            if (Enum.TryParse<BracketSpacing>(args.BracketSpacing, true, out var spacing))
                options.BracketSpacing = spacing;
            else
                throw new ArgumentException($"Invalid BracketSpacing value: {args.BracketSpacing}. Valid values are: NoSpaces, WithSpaces");
        }

        options.Validate();
        return options;
    }
}
