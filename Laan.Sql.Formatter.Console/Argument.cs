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

        if (args.Length == 0)
        {
            Console.WriteLine("usage: sqlformat.exe -File (file) -Sql (sql) [-Output (output)] [-Diagnostics]");
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
                SetValue(property.PropertyType != typeof(string) ? typeConverter.ConvertFromString(arg) : arg);
            }
        }
    }

    public bool Diagnostics { get; set; }
    public string File { get; set; }
    public string Output { get; set; }
    public string Sql { get; set; }
}
