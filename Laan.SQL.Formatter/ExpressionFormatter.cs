using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    internal class ExpressionFormatter
    {
        private int _indentStep;
        private string _indent;

        public ExpressionFormatter( string indent, int indentStep )
        {
            _indentStep = indentStep;
            _indent = indent;        
        }

        internal string GetIndent( string indent, int indentStep, bool includeNewLine )
        {
            string newLine = includeNewLine ? "\r\n" : "";
            StringBuilder result = new StringBuilder( newLine );
            for ( int index = 0; index < indentStep; index++ )
                result.Append( indent );
            return result.ToString();
        }

        internal string GetIndent( string indent, int indentStep )
        {
            return GetIndent( indent, indentStep, true );
        }

        private string FormatCaseElseExpression( int offset, CaseExpression caseSwitch )
        {
            return String.Format(
                "{0}ELSE{1}{2}",
                GetIndent( _indent, _indentStep ),
                GetIndent( _indent, _indentStep + 1 ),
                caseSwitch.Else.FormattedValue( offset, _indent, _indentStep )
            );
        }   
     
        internal string GetBooleanExpression( CriteriaExpression expr, int offset )
        {
            return String.Format(
                "{0}{1}{2}{3} {4}",
                expr.Left.FormattedValue( offset, _indent, _indentStep ),
                GetIndent( _indent, _indentStep ),
                new string( ' ', Math.Max( 0, offset - expr.Operator.Length ) ),
                expr.Operator,
                expr.Right.FormattedValue( offset, _indent, _indentStep )
            );
        }


        internal string FormatCaseSwitchExpression( Expression expr, int offset )
        {
            var caseSwitch = (CaseSwitchExpression) expr;
            var sql = new StringBuilder(
                String.Format(
                    "CASE {0}",
                    caseSwitch.Switch.FormattedValue( offset, _indent, _indentStep )
                )
            );

            foreach ( var caseItem in caseSwitch.Cases )
            {
                sql.Append(
                    String.Format(
                        "{0}WHEN {1} THEN {2}",
                        GetIndent( _indent, _indentStep + 1 ),
                        caseItem.When.FormattedValue( offset, _indent + 1, _indentStep ),
                        caseItem.Then.FormattedValue( offset, _indent + 1, _indentStep )
                    )
                );
            }
            if ( caseSwitch.Else != null )
                sql.Append( FormatCaseElseExpression( offset, caseSwitch ) );

            sql.Append( GetIndent( _indent, _indentStep ) + "END" );

            return sql.ToString();
        }

        internal string FormatCaseWhenExpression( Expression expr, int offset )
        {
            var caseSwitch = (CaseWhenExpression) expr;
            var sql = new StringBuilder( "CASE" );

            foreach ( var caseItem in caseSwitch.Cases )
            {
                sql.Append(
                    String.Format(
                        "{0}WHEN {1} THEN {2}",
                        GetIndent( _indent, _indentStep + 1 ),
                        caseItem.When.FormattedValue( offset, _indent + 1, _indentStep ),
                        caseItem.Then.FormattedValue( offset, _indent + 1, _indentStep )
                    )
                );
            }
            if ( caseSwitch.Else != null )
                sql.Append( FormatCaseElseExpression( offset, caseSwitch ) );

            sql.Append( GetIndent( _indent, _indentStep ) + "END" );

            return sql.ToString();
        }

        public string FormatNestedExpression( NestedExpression expr, int offset )
        {
            StringBuilder sql = new StringBuilder( GetIndent( _indent, _indentStep, false ) + "(" );
            sql.AppendLine();
            sql.AppendLine();
            sql.Append( expr.Expression.FormattedValue( offset, _indent, _indentStep ) );
            sql.AppendLine( GetIndent( _indent, _indentStep ) );
            sql.Append( GetIndent( _indent, _indentStep, false ) + ")" );
            return sql.ToString();
        }
    }
}