using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public static class ExpressionFormatter
    {
        private const string NEW_LINE = "\r\n";

        public static string FormattedValue( this Expression expr, int offset, string indent, int indentStep )
        {
            if ( expr is CriteriaExpression )
            {
                CriteriaExpression opExpr = (CriteriaExpression) expr;

                if ( opExpr.Operator == "AND" || opExpr.Operator == "OR" )
                    return
                        String.Format(
                            "{0}{1}{2}{3} {4}",
                            opExpr.Left.FormattedValue( offset, indent, indentStep ),
                            GetIndent( indent, indentStep ),
                            new string( ' ', Math.Max( 0, offset - opExpr.Operator.Length ) ),
                            opExpr.Operator,
                            opExpr.Right.FormattedValue( offset, indent, indentStep )
                        );
            }
            return expr.Value;
        }

        private static string GetIndent( string indent, int indentStep )
        {
            StringBuilder result = new StringBuilder( "\r\n" );
            for ( int index = 0; index < indentStep; index++ )
                result.Append( indent );
            return result.ToString();
        }
    }
}
