using System;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class SetVariableStatementFormatter : StatementFormatter<SetVariableStatement>, IStatementFormatter
    {
        public SetVariableStatementFormatter(IIndentable indentable, StringBuilder sql, SetVariableStatement statement)
            : base(indentable, sql, statement)
        {
        }

        public void Execute()
        {
            var variableAssignment = String.Format(
                "{0} {1} = {2}",
                Keyword(Constants.Set),
                _statement.Variable,
                _statement.Assignment.FormattedValue(0, this)
            );

            IndentAppend(variableAssignment);
            FormatTerminator();
        }
    }
}
