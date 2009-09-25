using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class SelectStatementFormatter : CustomFormatter<SelectStatement>, IStatementFormatter
    {
        private bool _indentSelect;
        private const int HavingLength = 6;
        private const int Padding = 4;

        public SelectStatementFormatter( string indent, int indentStep, StringBuilder sql, SelectStatement statement )
            : this( indent, indentStep, sql, statement, false )
        {
        }

        public SelectStatementFormatter( string indent, int indentStep, StringBuilder sql, SelectStatement statement, bool indentSelect )
            : base( indent, indentStep, sql, statement )
        {
            _indentSelect = indentSelect;
        }

        private void FormatSelect()
        {
            if ( _indentSelect )
                IndentedAppend( "SELECT" );
            else
                _sql.Append( "SELECT" );

            if ( _statement.Distinct )
                _sql.Append( " DISTINCT " );

            FormatTop( _statement.Top );
            FormatFields( _statement.Fields );
        }

        
        private void FormatFields( List<Field> fields )
        {
            if ( fields.Count == 1 && fields[ 0 ].Expression.Value.Length < 20 )
                _sql.Append( " " + fields[ 0 ].Expression.FormattedValue( 0, _indent, _indentStep ) );
            else
            {
                int count = fields.Count;
                foreach ( var field in fields )
                {
                    NewLine();
                    IndentedAppendFormat(
                        "{0}{1}{2}{3}{4}",
                        new string( ' ', Padding ),
                        field.Alias.Type == AliasType.Equals ? field.Alias.Value : "",
                        field.Expression.FormattedValue( 0, _indent, _indentStep + 1 ) + field.Value,
                        field.Alias.Type == AliasType.As ? field.Alias.Value : "",
                        ( --count > 0 ? "," : "" )
                    );
                }
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
                    IndentedAppendFormat(
                        "HAVING {0}",
                        _statement.Having.FormattedValue( HavingLength, _indent, _indentStep )
                    );
                }
            }
        }

        public void Execute()
        {
            FormatSelect();
            FormatFrom();
            FormatJoins();
            FormatWhere();
            FormatOrderBy();
            FormatGroupBy();
            FormatTerminator();
        }

    }
}
