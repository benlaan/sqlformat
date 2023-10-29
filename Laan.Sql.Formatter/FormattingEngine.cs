using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class FormattingEngine : IFormattingEngine
    {
        public FormattingEngine()
        {
            UseTabChar = false;
            TabSize = 4;
        }

        public string Execute(string sql)
        {
            var statements = ParserFactory.Execute(sql);
            return Execute(statements.ToArray());
        }

        private string Execute(params IStatement[] statements)
        {
            var outSql = new StringBuilder(1024);

            var indentation = new Indentation(TabSize, UseTabChar);

            foreach (var statement in statements)
            {
                var formatter = StatementFormatterFactory.GetFormatter(indentation, outSql, statement);
                formatter.Execute();

                if (statement != statements.Last())
                    outSql.AppendLine(Environment.NewLine);
            }

            return outSql.ToString();
        }

        public int IndentStep { get; set; }
        public int TabSize { get; set; }
        public bool UseTabChar { get; set; }
    }
}
