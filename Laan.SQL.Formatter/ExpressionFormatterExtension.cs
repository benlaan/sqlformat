using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public static class ExpressionFormatterExtension
    {
        public static string FormattedValue( this Expression expr, int offset, string indent, int indentStep )
        {
            var impl = new ExpressionFormatter( indent, indentStep );
            if ( expr is CriteriaExpression )
            {
                CriteriaExpression opExpr = (CriteriaExpression) expr;

                if ( opExpr.Operator == Constants.And || opExpr.Operator == Constants.Or )
                    return impl.GetBooleanExpression( opExpr, offset );
                else
                    return
                        opExpr.Left.FormattedValue( offset, indent, indentStep ) +
                        " " + opExpr.Operator + " " + 
                        opExpr.Right.FormattedValue( offset, indent, indentStep );
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
                var formatter = new SelectStatementFormatter( indent, indentStep + 1, sql, selectExpression.Statement );
                formatter.Execute();
                return sql.ToString();
            }
            return expr.Value;
        }
    }
}
