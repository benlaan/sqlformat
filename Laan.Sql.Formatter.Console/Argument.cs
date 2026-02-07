using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

public class Argument
{
    /// <summary>
    /// Parses command line argument in the form -File (file) -Sql (sql) [-Output (output)] [-Diagnostic]
    /// </summary>
    /// <param name="args"></param>
    public Argument(string[] args)
    {
        PropertyInfo property = null;
        var typeConverter = new TypeConverter();

        void SetValue(object value)
        {
            property.SetValue(this, value);
            property = null;
        }

        if (args.Length == 0 && !Console.IsInputRedirected)
        {
            Console.WriteLine("usage: sqlformat -File (file) -Sql (sql) [-Output (output)] [-Diagnostics]");
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
            if (property == null)
            {
                if (arg.StartsWith("-"))
                {
                    property = GetType().GetProperties().SingleOrDefault(p => String.Compare(p.Name, arg.Trim('-'), StringComparison.OrdinalIgnoreCase) == 0);

                    if (property?.PropertyType == typeof(bool))
                        SetValue(true);
                }
            }
            else
            {
                var value = arg;
                if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?))
                    SetValue(int.Parse(value));
                else if (property.PropertyType == typeof(bool))
                    SetValue(bool.Parse(value));
                else
                    SetValue(value);
            }
        }
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
