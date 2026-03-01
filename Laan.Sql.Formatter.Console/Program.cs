using System;
using System.IO;
using System.Text;

using Laan.Sql.Formatter;

public static class Program
{
    private static void Main(string[] args)
    {
        var engine = new FormattingEngine();

        var timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        try
        {
            string sql = null;

            var argument = new Argument(args);

            if (Console.IsInputRedirected)
            {
                sql = Console.In.ReadToEnd();
            }
            else
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("usage: sqlformat.exe -File (file) -Sql (sql) [-Output (output)] [-Diagnostics]");
                    Environment.Exit(1);
                    return;
                }

                if (argument.Sql != null)
                    sql = argument.Sql;
                else
                    if (argument.File != null && File.Exists(argument.File))
                    sql = File.ReadAllText(argument.File);
            }

            if (sql == null)
            {
                Console.WriteLine("No SQL found - either supply -Sql or -File arguments, or via stdin");
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