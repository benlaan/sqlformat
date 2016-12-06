using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class ExecStatementFormatter : StatementFormatter<ExecStatement>, IStatementFormatter
    {
        public ExecStatementFormatter(IIndentable indentable, StringBuilder sql, ExecStatement statement) : base(indentable, sql, statement)
        {
        }

        private static string GetArgumentFormatted(Argument argument)
        {
            return !String.IsNullOrEmpty(argument.Name) ? String.Format("{0} = {1}", argument.Name, argument.Value) : argument.Value.ToString();
        }

        public void Execute()
        {
            IndentAppendFormat("{0} {1}", Constants.Exec, _statement.FunctionName);

            if (!_statement.Arguments.Any())
                return;

            var inline = String.Join(", ", _statement.Arguments.Select(a => GetArgumentFormatted(a)).ToArray());

            if (FitsOnRow(inline))
                IndentAppend(" " + inline);
            else
            {
                IndentAppendLine(String.Empty);
                using (new IndentScope(this))
                {
                    var lastArgument = _statement.Arguments.Last();
                    foreach (var argument in _statement.Arguments)
                    {
                        var lineEnding = (argument != lastArgument ? ",\n\r" : "");
                        IndentAppend(GetArgumentFormatted(argument) + lineEnding);
                    }
                }
            }
        }

    }
}
