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
}
