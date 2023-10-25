using System;
using System.Runtime.InteropServices.JavaScript;

using Laan.Sql.Formatter;

public static class Program
{
    public static void Main()
    {
    }
}

public partial class Formatter
{
    [JSExport]
    internal static string Execute(string sql)
    {
        var engine = new FormattingEngine();
        return engine.Execute(sql);
    }

    [JSImport("node.process.version", "main.mjs")]
    internal static partial string GetNodeVersion();
}
