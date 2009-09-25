using System;
using System.Text;
using System.Linq;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class CustomFormatter<T> where T : CustomStatement
    {
        public CustomFormatter( string indent, int indentStep, StringBuilder sql, T statement  )
        {
            _indent = indent;
            _indentStep = indentStep; 
            _sql = sql;
            _statement = statement;
        }

        private const int WhereLength = 5;

        protected string _indent;
        protected int _indentStep;
        protected StringBuilder _sql;
        protected T _statement;

        protected void IndentedAppend( string text )
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

        protected void IndentedAppendFormat( string text, params object[] args )
        {
            IndentedAppend( String.Format( text, args ) );
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
                NewLine();
                foreach ( var from in _statement.From )
                {
                    if ( from is DerivedTable )
                    {
                        DerivedTable derivedTable = (DerivedTable) from;
                        var formatter = new SelectStatementFormatter( _indent, _indentStep + 1, _sql, derivedTable.SelectStatement, true );
                        NewLine();
                        IndentedAppend( "FROM (" );
                        NewLine( 2 );
                        formatter.Execute();
                        NewLine( 2 );
                        IndentedAppend( String.Format( "){0}", from.Alias.Value ) );
                    }
                    else
                    {
                        NewLine();
                        IndentedAppendFormat(
                            "FROM {0}{1}",
                            from.Name, from.Alias.Value
                        );
                    }
                }
            }
        }

        protected void FormatJoins()
        {
            if ( _statement.Joins != null && _statement.Joins.Any() )
            {
                foreach ( var join in _statement.Joins )
                {
                    if ( join is DerivedJoin )
                    {
                        DerivedJoin derivedJoin = (DerivedJoin) join;
                        
                        var formatter = new SelectStatementFormatter(
                            _indent, 
                            _indentStep + 1, 
                            _sql, 
                            derivedJoin.SelectStatement,
                            true 
                        );

                        NewLine( 2 );
                        IndentedAppend( join.Value );
                        NewLine( 2 );
                        formatter.Execute();
                        NewLine( 2 );
                        IndentedAppend( String.Format( "){0}", join.Alias.Value ) );
                        NewLine();
                        IndentedAppendFormat(
                            "  ON {0}",
                            join.Condition.FormattedValue( 4, _indent, _indentStep )
                        );
                    }
                    else
                    {
                        NewLine( 2 );
                        IndentedAppend( join.Value );
                        NewLine();
                        IndentedAppendFormat(
                            "{0}ON {1}",
                            new string( ' ', join.Length - "ON".Length ),
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
                NewLine( 2 );
                IndentedAppendFormat( 
                    "{0} {1}", 
                    Constants.Where, _statement.Where.FormattedValue( WhereLength, _indent, _indentStep ) 
                );
            }
        }

        protected void FormatTerminator()
        {
            if ( _statement.Terminated )
                _sql.Append( Constants.SemiColon );
        }
    }
}
