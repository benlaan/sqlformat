using System;
using System.Text;
using System.Linq;

using Laan.SQL.Parser;
using System.Collections.Generic;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public enum BracketFormatOption
    {
        NoSpaces,
        SpacesWithinBrackets
    }

    public class CustomFormatter<T> : IIndentable where T : CustomStatement
    {
        BracketFormatOption bracketSpaceOption = BracketFormatOption.NoSpaces;
        private const int WrapMarginColumn = 80;

        private static Dictionary<BracketFormatOption, string> _bracketFormats;

        protected int _indentStep;

        static CustomFormatter()
        {
            _bracketFormats = new Dictionary<BracketFormatOption, string>()
            {
                { BracketFormatOption.NoSpaces, "({0})" },
                { BracketFormatOption.SpacesWithinBrackets, "( {0} )" },
            };
        }

        public CustomFormatter( string indent, int indentStep, StringBuilder sql, T statement )
        {
            _indent = indent;
            _indentStep = indentStep;
            _sql = sql;
            _statement = statement;
        }

        private const int WhereLength = 5;

        protected string _indent;
        protected StringBuilder _sql;
        protected T _statement;

        protected void Append( string text )
        {
            for ( int count = 0; count < _indentStep; count++ )
                _sql.Append( _indent );
            _sql.Append( text );
        }

        protected void NewLine( int times )
        {
            for ( int index = 0; index < times; index++ )
                _sql.AppendLine();
        }

        protected void NewLine()
        {
            NewLine( 1 );
        }

        protected void AppendFormat( string text, params object[] args )
        {
            Append( String.Format( text, args ) );
        }

        protected void FormatTop( Top top )
        {
            if ( top == null )
                return;

            string format = top.Brackets ? " TOP ({0}){1}" : " TOP {0}{1}";

            _sql.Append(
                String.Format(
                    format,
                    top.Expression.FormattedValue( 0, _indent, _indentStep ),
                    top.Percent ? " PERCENT" : ""
                )
            );
        }

        protected void FormatFrom()
        {
            if ( _statement.From != null && _statement.From.Any() )
            {
                NewLine( CanCompactFormat() ? 0 : 1 );
                foreach ( var from in _statement.From )
                {
                    if ( from is DerivedTable )
                    {
                        DerivedTable derivedTable = (DerivedTable) from;
                        var formatter = new SelectStatementFormatter( _indent, _indentStep + 1, _sql, derivedTable.SelectStatement, true );
                        NewLine();
                        Append( "FROM (" );
                        NewLine( CanCompactFormat() ? 1 : 2 );
                        formatter.Execute();
                        NewLine( CanCompactFormat() ? 1 : 2 );
                        Append( String.Format( "){0}", from.Alias.Value ) );
                    }
                    else
                    {
                        NewLine();
                        AppendFormat(
                            "FROM {0}{1}",
                            from.Name, from.Alias.Value
                        );
                    }
                }
            }
        }

        private void FormatDerivedJoin( DerivedJoin derivedJoin )
        {
            var formatter = new SelectStatementFormatter(
                _indent,
                _indentStep + 1,
                _sql,
                derivedJoin.SelectStatement,
                true
            );

            NewLine( 2 );
            Append( derivedJoin.Value );
            NewLine( 2 );
            formatter.Execute();
            NewLine( 2 );
            Append( String.Format( "){0}", derivedJoin.Alias.Value ) );
            NewLine();
            AppendFormat(
                "  ON {0}",
                derivedJoin.Condition.FormattedValue( 4, _indent, _indentStep )
            );
        }

        protected void FormatJoins()
        {
            if ( _statement.Joins != null && _statement.Joins.Any() )
            {
                foreach ( var join in _statement.Joins )
                {
                    if ( join is DerivedJoin )
                        FormatDerivedJoin( (DerivedJoin) join );
                    else
                    {
                        NewLine( 2 );
                        Append( join.Value );
                        NewLine();
                        AppendFormat(
                            "{0}ON {1}",
                            new string( ' ', join.Length - Constants.On.Length ),
                            join.Condition.FormattedValue( join.Length, _indent, _indentStep )
                        );
                    }
                }
            }
        }

        protected void FormatWhere()
        {
            if ( _statement.Where != null )
            {
                NewLine( CanCompactFormat() ? 1 : 2 );
                AppendFormat(
                    "{0} {1}",
                    Constants.Where,
                    _statement.Where.FormattedValue( WhereLength, _indent, _indentStep )
                );
            }
        }

        protected void FormatTerminator()
        {
            if ( _statement.Terminated )
                _sql.Append( Constants.SemiColon );
        }

        protected bool FitsOnRow( string text )
        {
            return text.Length <= ( WrapMarginColumn - ( _indentStep * _indent.Length + CurrentColumn ) );
        }

        protected int CurrentColumn
        {
            get { return _sql.ToString().Split( '\n' ).Last().Length; }
        }

        protected string FormatBrackets( string text )
        {
            return String.Format( _bracketFormats[ bracketSpaceOption ], text );
        }

        protected bool IsExpressionOperatorAndOr( Expression expression )
        {
            CriteriaExpression where = expression as CriteriaExpression;
            return where == null || ( where.Operator != Constants.And && where.Operator != Constants.Or );
        }

        protected virtual bool CanCompactFormat()
        {
            return IsExpressionOperatorAndOr( _statement.Where );
        }

        public void Indent()
        {
            _indentStep++;
        }

        public void Unindent()
        {
            _indentStep--;
        }
        
    }
}
