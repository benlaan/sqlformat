using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public static class ExpressionFormatterExtension
    {
        public static string FormattedValue( this Expression expr, int offset, string indent, int indentLevel )
        {
            var impl = new ExpressionFormatter( indent, indentLevel );
            if ( expr is CriteriaExpression )
            {
                CriteriaExpression opExpr = (CriteriaExpression) expr;

                if ( opExpr.Operator == Constants.And || opExpr.Operator == Constants.Or )
                    return impl.GetBooleanExpression( opExpr, offset );
                else
                    return
                        opExpr.Left.FormattedValue( offset, indent, indentLevel ) +
                        " " + opExpr.Operator + " " + 
                        opExpr.Right.FormattedValue( offset, indent, indentLevel );
            }

            if ( expr is CaseSwitchExpression )
                return impl.FormatCaseSwitchExpression( expr, offset );

            if ( expr is CaseWhenExpression )
                return impl.FormatCaseWhenExpression( expr, offset );

            if ( expr is NestedExpression )
            {
                return impl.FormatNestedExpression( expr as NestedExpression, offset );
            }

            if ( expr is SelectExpression )
            {
                SelectExpression selectExpression = (SelectExpression) expr;
                var sql = new StringBuilder();
                var formatter = new SelectStatementFormatter( indent, indentLevel + 1, sql, selectExpression.Statement );
                formatter.Execute();
                return sql.ToString();
            }
            return expr.Value;
        }
    }
}
