using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class CreateViewStatementFormatter : StatementFormatter<CreateViewStatement>, IStatementFormatter
    {
        public CreateViewStatementFormatter(IIndentable indentable, StringBuilder sql, CreateViewStatement statement)
            : base(indentable, sql, statement)
        {
        }

        public void Execute()
        {
            _sql.AppendFormat("{0} VIEW {1}\n", _statement.IsAlter ? "ALTER" : "CREATE", _statement.Name);
            _sql.AppendLine("AS");

            if (_statement.Definition is SelectStatement)
            {
                var formatter = new SelectStatementFormatter(this, _sql, (SelectStatement)_statement.Definition);
                formatter.Execute();
            }
            else if (_statement.Definition is CommonTableExpressionStatement)
            {
                var formatter = new CommonTableExpressionStatementFormatter(this, _sql, (CommonTableExpressionStatement)_statement.Definition);
                formatter.Execute();
            }
        }
    }
}
