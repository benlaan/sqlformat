using System;
using System.IO;
using System.Text;

using Laan.Sql.Formatter;

internal static class Program
{
    // TODO: Implement a proper argument class for passing SQL, or a file, as well as output
    private static void Main(string[] args)
    {
        var argument = new Argument(args);

        // Load formatting options
        var options = LoadOptions(argument);
        var engine = new FormattingEngine(options);

        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        try
        {
            string sql = null;

            if (argument.Sql != null)
                sql = argument.Sql;
            else if (argument.File != null && File.Exists(argument.File))
                sql = File.ReadAllText(argument.File);
            else if (Console.IsInputRedirected)
            {
                // Read from stdin if input is redirected (piped)
                using (var reader = Console.In)
                {
                    sql = reader.ReadToEnd();
                }
            }

            if (sql == null)
            {
                Console.WriteLine("No SQL found - either supply -Sql or -File arguments, or pipe SQL via stdin");
                Environment.Exit(2);
                return;
            }

            var output = engine.Execute(sql);

            if (argument.Output != null)
            {
                File.WriteAllText(argument.Output, output, Encoding.UTF8);
            }
            else
            {
                var formattedSql = output.TrimEnd().Split(new[] { "\r\n" }, StringSplitOptions.None);
                foreach (var line in formattedSql)
                    Console.WriteLine(line);
            }

            if (argument.Diagnostics)
                Console.WriteLine("\nElapsed Time: " + TimeSpan.FromMilliseconds(timer.ElapsedMilliseconds));

            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            Environment.Exit(3);
        }
        finally
        {
            timer.Stop();
        }
    }

    private static FormattingOptions LoadOptions(Argument argument)
    {
        FormattingOptions options;

        // Load from config file if specified, otherwise search hierarchy
        if (!string.IsNullOrEmpty(argument.ConfigFile))
        {
            options = FormattingOptionsLoader.LoadFromFile(argument.ConfigFile);
        }
        else
        {
            // Try to load from hierarchy (current dir -> parent dirs -> home dir)
            options = FormattingOptionsLoader.TryLoadFromHierarchy();
        }

        // Override with command-line arguments
        if (argument.IndentSize.HasValue)
            options.IndentSize = argument.IndentSize.Value;

        if (argument.UseTabs)
            options.UseSpaces = false;
        else if (argument.UseSpaces)
            options.UseSpaces = true;

        if (argument.MaxLineLength.HasValue)
            options.MaxLineLength = argument.MaxLineLength.Value;

        if (!string.IsNullOrEmpty(argument.KeywordCasing))
        {
            if (Enum.TryParse<KeywordCasing>(argument.KeywordCasing, true, out var casing))
                options.KeywordCasing = casing;
            else
                throw new ArgumentException($"Invalid KeywordCasing value: {argument.KeywordCasing}. Valid values are: Upper, Lower, Pascal");
        }

        if (!string.IsNullOrEmpty(argument.BracketSpacing))
        {
            if (Enum.TryParse<BracketSpacing>(argument.BracketSpacing, true, out var spacing))
                options.BracketSpacing = spacing;
            else
                throw new ArgumentException($"Invalid BracketSpacing value: {argument.BracketSpacing}. Valid values are: NoSpaces, WithSpaces");
        }

        options.Validate();
        return options;
    }
}