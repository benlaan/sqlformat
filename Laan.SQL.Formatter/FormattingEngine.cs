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
        }

        public string Execute(string sql)
        {
            var outSql = new StringBuilder(sql.Length * 2);
            var statements = ParserFactory.Execute(sql);

            var indentation = new Indentation();

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
