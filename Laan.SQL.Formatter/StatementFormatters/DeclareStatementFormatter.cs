using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class DeclareStatementFormatter : StatementFormatter<DeclareStatement>, IStatementFormatter
    {
        public DeclareStatementFormatter(IIndentable indentable, StringBuilder sql, DeclareStatement statement)
            : base(indentable, sql, statement)
        {
        }

        public void Execute()
        {
            IndentAppend("DECLARE");

            var formatter = new VariableDefinitionFormatter(_statement.Definitions, this, _sql);
            formatter.Execute();
        }
    }
}
