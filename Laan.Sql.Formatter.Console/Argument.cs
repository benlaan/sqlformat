using System;
using System.ComponentModel;
using System.Linq;

public class Argument
{
    /// <summary>
    /// Parses command line argument in the form -File (file) -Sql (sql) [-Output (output)] [-Diagnostic]
    /// </summary>
    /// <param name="args"></param>
    public Argument(string[] args)
    {
        if (args.Length == 0 && !Console.IsInputRedirected)
        {
            Console.WriteLine("usage: sqlformat.exe -File (file) -Sql (sql) [-Output (output)] [-Diagnostics]");
            Console.WriteLine("   or: cat file.sql | sqlformat [-Output (output)] [-Diagnostics]");
            Console.WriteLine("");
            Console.WriteLine("Formatting Options:");
            Console.WriteLine("  -IndentSize (number)       Number of spaces per indent level (default: 4)");
            Console.WriteLine("  -UseSpaces                 Use spaces for indentation (default: true)");
            Console.WriteLine("  -UseTabs                   Use tabs for indentation");
            Console.WriteLine("  -MaxLineLength (number)    Maximum line length before wrapping (default: 80)");
            Console.WriteLine("  -KeywordCasing (style)     Keyword casing: Upper, Lower, or Pascal (default: Upper)");
            Console.WriteLine("  -BracketSpacing (style)    Bracket spacing: NoSpaces or WithSpaces (default: NoSpaces)");
            Console.WriteLine("  -ConfigFile (path)         Path to .sqlformat.json config file");
            Console.WriteLine("");
            Console.WriteLine("If no options are specified, searches for .sqlformat.json in current/parent directories.");
            Environment.Exit(1);
            return;
        }
        foreach (string arg in args)
        {
            if (arg.StartsWith("-"))
            {
                var propertyName = arg.Trim('-');
                switch (propertyName.ToLower())
                {
                    case "file":
                        File = ReadValue(args, arg);
                        break;

                    case "sql":
                        Sql = ReadValue(args, arg);
                        break;

                    case "output":
                        Output = ReadValue(args, arg);
                        break;

                    case "diagnostics":
                        Diagnostics = true;
                        break;

                    case "indentsize":
                        var indentValue = ReadValue(args, arg);
                        if (int.TryParse(indentValue, out var indent))
                            IndentSize = indent;
                        break;

                    case "usespaces":
                        UseSpaces = true;
                        break;

                    case "usetabs":
                        UseTabs = true;
                        break;

                    case "maxlinelength":
                        var lengthValue = ReadValue(args, arg);
                        if (int.TryParse(lengthValue, out var length))
                            MaxLineLength = length;
                        break;

                    case "keywordcasing":
                        KeywordCasing = ReadValue(args, arg);
                        break;

                    case "bracketspacing":
                        BracketSpacing = ReadValue(args, arg);
                        break;

                    case "configfile":
                        ConfigFile = ReadValue(args, arg);
                        break;
                }
            }
        }
    }

    private string ReadValue(string[] args, string currentArg)
    {
        var currentIndex = Array.IndexOf(args, currentArg);
        if (currentIndex < 0 || currentIndex >= args.Length - 1)
            return null;

        return args[currentIndex + 1];
    }

    public bool Diagnostics { get; set; }
    public string File { get; set; }
    public string Output { get; set; }
    public string Sql { get; set; }
    
    // Formatting options
    public int? IndentSize { get; set; }
    public bool UseSpaces { get; set; }
    public bool UseTabs { get; set; }
    public int? MaxLineLength { get; set; }
    public string KeywordCasing { get; set; }
    public string BracketSpacing { get; set; }
    public string ConfigFile { get; set; }
}
