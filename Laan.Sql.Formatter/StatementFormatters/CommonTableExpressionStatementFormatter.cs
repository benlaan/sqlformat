using System;
using System.Linq;
using System.Text;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class CommonTableExpressionStatementFormatter : StatementFormatter<CommonTableExpressionStatement>, IStatementFormatter
    {
        public CommonTableExpressionStatementFormatter(IIndentable indentable, StringBuilder sql, CommonTableExpressionStatement statement)
            : base(indentable, sql, statement)
        {
        }

        public void Execute()
        {
            _sql.Append("WITH ");

            foreach (var commonTableExpression in _statement.CommonTableExpressions)
            {
                _sql.AppendFormat("{0} AS ({1}{1}", commonTableExpression.Name, Environment.NewLine);

                using (new IndentScope(this))
                {
                    var cteFormatter = new SelectStatementFormatter(this, _sql, commonTableExpression);
                    cteFormatter.Execute();

                    _sql.AppendFormat(
                        "{1}){0}{1}", 
                        (commonTableExpression != _statement.CommonTableExpressions.Last() ? "," : String.Empty), 
                        Environment.NewLine
                    );
                }
            }

            var formatter = new SelectStatementFormatter(this, _sql, _statement.Statement);
            formatter.Execute();
        }
    }
}
