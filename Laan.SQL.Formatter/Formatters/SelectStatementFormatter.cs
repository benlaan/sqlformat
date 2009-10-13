using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.SQL.Parser;
using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public class SelectStatementFormatter : CustomFormatter<SelectStatement>, IStatementFormatter
    {
        private const int Padding = 4;
        private const int MaxInlineColumns = 1;

        private bool _indentSelect;

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
                Append( Constants.Select );
            else
                _sql.Append( Constants.Select );

            if ( _statement.Distinct )
                _sql.Append( " DISTINCT " );

            FormatTop( _statement.Top );
            FormatFields(_statement.Fields, CanCompactFormat());
        }

/*
        private string GetFieldValue( Field field )
        {
            var offset = field.Alias.Type == AliasType.Equals ? field.Alias.Name.Length : 0;
            return field.Expression.FormattedValue( offset, _indent, _indentStep ) + field.Value;
        }

        private void FormatField( Field field, int maxValueLength, bool last )
        {
            string result = "";
            string nameFormat = String.Format( "{{0,{0}}}", -1 * maxValueLength );
            string fieldValue = GetFieldValue(field );
            switch ( field.Alias.Type )
            {
                case AliasType.As:
                case AliasType.Implicit:
                    string fieldName = String.Format( nameFormat, fieldValue );
                    result = String.Format(
                                "{0}{1}{2}",
                                fieldName,
                                ( field.Alias.Type == AliasType.As ? " AS" : "" ),
                                field.Alias.Name != null ? " " + field.Alias.Name : ""
                            );
                    break;

                case AliasType.Equals:
                    fieldName = String.Format( nameFormat, field.Alias.Name );
                    result = String.Format( "{0} = {1}", fieldName, fieldValue );
                    break;

                case AliasType.None:
                    result = fieldValue;
                    break;
            }

            NewLine();
            Append( result.Trim() + ( last ? "" : "," ) );
        }

        private int GetLeftSideLength( Field field )
        {
            var fieldValue = GetFieldValue( field );
            var fieldName = field.Alias != null ? field.Alias.Name : "";

            switch ( field.Alias.Type )
            {
                case AliasType.As:
                case AliasType.Implicit:
                case AliasType.None:
                    string[] lines = fieldValue.Split( new[] { "\r\n" }, StringSplitOptions.None );
                    return lines.Max( line => line.Length );

                case AliasType.Equals:
                    return fieldName.Length;

                default:
                    return 0;
            }
        }

        private void FormatFields( List<Field> fields )
        {
            if ( fields.Count == 1 && fields[ 0 ].Expression.Value.Length < 20 )
                _sql.Append( " " + fields[ 0 ].Expression.FormattedValue( 0, _indent, _indentStep ) );
            else
            {
                _lines = fields.Select( 
                    field => GetFieldValue( field ).Split( new[] { "\r\n" }, StringSplitOptions.None ) 
                ).ToList();

                int maxValueLength = fields.Max( f => GetLeftSideLength( f ) );

                using ( new IndentScope( this ) )
                {
                    foreach ( var field in fields )
                        FormatField( field, maxValueLength, field == fields.Last() );
                }
            }
        }
*/

        private void FormatFields( List<Field> fields, bool canCompact )
        {
            if ( fields.Count <= MaxInlineColumns && fields[ 0 ].Expression.CanInline && FitsOnRow( fields[ 0 ].Expression.Value ) )
                _sql.Append( " " + fields[ 0 ].Expression.FormattedValue( 0, _indent, _indentStep ) );
            else
                using ( new IndentScope( this ) )
                {
                    foreach ( var field in fields )
                    {
                        NewLine( canCompact && field.Expression.CanInline ? 0 : 1 );
                        AppendFormat(
                            "{0}{1}{2}{3}",
                            field.Alias.Type == AliasType.Equals ? field.Alias.Value : "",
                            field.Expression.FormattedValue( 0, _indent, _indentStep ) + field.Value,
                            field.Alias.Type == AliasType.As ? field.Alias.Value : "",
                            ( field != fields.Last() ? "," : "" )
                        );
                    }
                }
        }

        private void FormatOrderBy()
        {
            if ( _statement.OrderBy.Count > 0 )
            {
                bool canCompact = _statement.OrderBy.Count <= MaxInlineColumns;
                NewLine( canCompact ? 1 : 2 );
                Append( "ORDER BY" );
                FormatFields( _statement.OrderBy, canCompact );
            }
        }

        private void FormatGroupBy()
        {
            if ( _statement.GroupBy.Count > 0 )
            {
                bool canCompact = _statement.GroupBy.Count <= MaxInlineColumns;
                NewLine( canCompact ? 1 : 2 );
                Append( "GROUP BY" );
                FormatFields( _statement.GroupBy, canCompact );

                if ( _statement.Having != null )
                {
                    NewLine( canCompact && IsExpressionOperatorAndOr( _statement.Having ) ? 1 : 2 );
                    AppendFormat(
                        "HAVING {0}",
                        _statement.Having.FormattedValue( Constants.Having.Length, _indent, _indentStep )
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

        protected override bool CanCompactFormat()
        {
            return
                _statement.Fields.Count <= MaxInlineColumns
                &&
                _statement.From.Count == 1 && _statement.Joins.Count == 0
                &&
                base.CanCompactFormat();

        }

    }
}
