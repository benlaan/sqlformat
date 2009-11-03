using System;
using System.Text;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class SelectExpressionFormatter : CustomExpressionFormatter<SelectExpression>
    {
        public SelectExpressionFormatter( SelectExpression expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            var sql = new StringBuilder();
            var formatter = new SelectStatementFormatter( this, sql, _expression.Statement );
            formatter.Execute();
            return sql.ToString();
        }

        #endregion
    }
}
