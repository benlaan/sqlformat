using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class SelectStatementFormatter : IStatementFormatter
    {
        private string _indent;
        private int _indentStep;

        private const int WhereLength = 5;
        private const int HavingLength = 6;
        private const int Padding = 4;

        private SelectStatement _statement;
        private StringBuilder _sql;

        public SelectStatementFormatter( string indent, int indentStep, StringBuilder sql, SelectStatement statement )
        {
            _indentStep = indentStep;
            _indent = indent;
            _statement = statement;
            _sql = sql;
        }

        private void IndentedAppend( string text )
        {
            for ( int count = 0; count < _indentStep; count++ )
                _sql.Append( _indent );
            _sql.Append( text );
        }

        private void NewLine( int times )
        {
            for ( int index = 0; index < times; index++ )
                _sql.AppendLine();
        }

        private void NewLine()
        {
            NewLine( 1 );
        }

        private void IndentedAppendFormat( string text, params object[] args )
        {
            IndentedAppend( String.Format( text, args ) );
        }

        private void FormatSelect()
        {
            IndentedAppend( "SELECT" );
            if ( _statement.Distinct )
                _sql.Append( " DISTINCT " );

            if ( _statement.Top.HasValue )
                _sql.Append( " TOP " + _statement.Top.Value.ToString() );

            FormatFields( _statement.Fields );
        }

        private void FormatFields( List<Field> fields )
        {
            if ( fields.Count == 1 )
                _sql.Append( " " + fields[ 0 ].Expression.Value );
            else
            {
                int count = fields.Count;
                foreach ( var field in fields )
                {
                    NewLine();
                    IndentedAppendFormat(
                        "{0}{1}{2}",
                        new string( ' ', Padding ),
                        field.Expression.Value + field.Value,
                        ( --count > 0 ? "," : "" )
                    );
                }
            }
        }

        private void FormatFrom()
        {
            if ( _statement.From != null && _statement.From.Any() )
            {
                NewLine();
                foreach ( var from in _statement.From )
                {
                    if ( from is DerivedTable )
                    {
                        DerivedTable derivedTable = (DerivedTable) from;
                        var formatter = new SelectStatementFormatter( _indent, _indentStep + 1, _sql, derivedTable.SelectStatement );
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

        private void FormatJoins()
        {
            if ( _statement.Joins != null && _statement.Joins.Any() )
            {
                foreach ( var join in _statement.Joins )
                {
                    if ( join is DerivedJoin )
                    {
                        DerivedJoin derivedJoin = (DerivedJoin) join;
                        var formatter = new SelectStatementFormatter( _indent, _indentStep + 1, _sql, derivedJoin.SelectStatement );
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

        private void FormatWhere()
        {
            if ( _statement.Where != null )
            {
                NewLine( 2 );
                IndentedAppendFormat( "WHERE {0}", _statement.Where.FormattedValue( WhereLength, _indent, _indentStep ) );
            }
        }

        private void FormatOrderBy()
        {
            if ( _statement.OrderBy.Count > 0 )
            {
                NewLine( 2 );
                IndentedAppend( "ORDER BY" );
                FormatFields( _statement.OrderBy );
            }
        }

        private void FormatGroupBy()
        {
            if ( _statement.GroupBy.Count > 0 )
            {
                NewLine( 2 );
                IndentedAppend( "GROUP BY" );
                FormatFields( _statement.GroupBy );

                if ( _statement.Having != null )
                {
                    NewLine( 2 );
                    IndentedAppendFormat( "HAVING {0}", _statement.Having.FormattedValue( HavingLength, _indent, _indentStep ) );
                }
            }
        }

        public string Execute()
        {
            FormatSelect();
            FormatFrom();
            FormatJoins();
            FormatWhere();
            FormatOrderBy();
            FormatGroupBy();
            return _sql.ToString();
        }
    }
}
