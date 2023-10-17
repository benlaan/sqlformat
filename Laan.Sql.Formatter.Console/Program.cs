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
        var engine = new FormattingEngine();

        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        try
        {
            string sql = null;

            if (argument.Sql != null)
                sql = argument.Sql;
            else
                if (argument.File != null && File.Exists(argument.File))
                sql = File.ReadAllText(argument.File);

            if (sql == null)
            {
                Console.WriteLine("No SQL found - either supply -Sql or -File arguments");
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
}