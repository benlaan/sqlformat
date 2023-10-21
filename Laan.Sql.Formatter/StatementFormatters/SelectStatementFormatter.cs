using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Formatter
{
    public class SelectStatementFormatter : CustomStatementFormatter<SelectStatement>, IStatementFormatter
    {
        private const int MaxInlineColumns = 1;

        public SelectStatementFormatter( IIndentable indentable, StringBuilder sql, SelectStatement statement )
            : base( indentable, sql, statement )
        {
        }

        private void FormatSelect()
        {
            IndentAppend( Constants.Select );
            if ( _statement.Distinct )
                _sql.Append( " DISTINCT " );

            FormatTop( _statement.Top );
            FormatFields( _statement.Fields, CanInline );
        }

/*
        private string GetFieldValue( Field field )
        {
            var offset = field.Alias.Type == AliasType.Equals ? field.Alias.Name.Length : 0;
            return field.Expression.FormattedValue( offset, this ) + field.Value;
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
                _sql.Append( " " + fields[ 0 ].Expression.FormattedValue( 0, this ) );
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

        private string FormatField( Field field )
        {
            return String.Concat(
                field.Alias.Type == AliasType.Equals ? field.Alias.Value : "",
                field.Expression.FormattedValue( 0, this ) + field.Value,
                ( field.Alias.Type == AliasType.As || field.Alias.Type == AliasType.Implicit ) ? field.Alias.Value : ""
            );
        }

        private void FormatFields( List<Field> fields, bool canCompact )
        {
            if ( fields.Count <= MaxInlineColumns && fields.Take( MaxInlineColumns ).All( f => f.Expression.CanInline && FitsOnRow( f.Expression.Value ) ) )
                _sql.Append( " " + String.Join( ", ", fields.Take( MaxInlineColumns ).Select( f => FormatField( f ) ).ToArray() ) );
            else
                using ( new IndentScope( this ) )
                {
                    foreach ( var field in fields )
                    {
                        NewLine( canCompact && field.Expression.CanInline ? 0 : 1 );
                        IndentAppend( FormatField( field ) + ( field != fields.Last() ? "," : "" ) );
                    }
                }
        }

        private void FormatInto()
        {
            if (_statement.Into != null )
            {
                NewLine( 2 );
                IndentAppend( "INTO " + _statement.Into );
            }
        }

        private void FormatGroupBy()
        {
            if ( _statement.GroupBy.Count > 0 )
            {
                bool canCompact = CanCompactFormat();
                NewLine( canCompact ? 1 : 2 );
                IndentAppend( "GROUP BY" );
                FormatFields( _statement.GroupBy, canCompact );

                if ( _statement.Having != null )
                {
                    NewLine( canCompact && IsExpressionOperatorAndOr( _statement.Having ) ? 1 : 2 );
                    IndentAppendFormat(
                        "HAVING {0}",
                        _statement.Having.FormattedValue( Constants.Having.Length, this )
                    );
                }
            }
        }

        private void FormatOrderBy()
        {
            if ( _statement.OrderBy.Count > 0 )
            {
                bool canCompact = CanCompactFormat();
                NewLine( canCompact ? 1 : 2 );
                IndentAppend( "ORDER BY" );
                FormatFields( _statement.OrderBy, canCompact );
            }
        }

        private void FormatSetOperation()
        {
            var map = new Dictionary<SetType, string> {
                { SetType.Union,     "UNION" },
                { SetType.UnionAll,  "UNION ALL" },
                { SetType.Intersect, "INTERSECT" },
                { SetType.Except,    "EXCEPT" },
            };

            NewLine( 2 );
            IndentAppend( map[ _statement.SetOperation.Type ] );
            NewLine( 2 );
            FormatStatement( _statement.SetOperation.Statement );
        }

        protected override bool CanCompactFormat()
        {
            return
                _statement.Fields.Count == 1 &&
                !_statement.From.SelectMany( fr => fr.Joins ).Any() && 
                _statement.OrderBy.Count <= MaxInlineColumns && 
                _statement.GroupBy.Count <= MaxInlineColumns && 
                base.CanCompactFormat();
        }

        public override bool CanInline
        {
            get { return _statement.CanInLine(); }
        }

        public void Execute()
        {
            //TODO: Need to implement Statement.Value
            //if ( CanInline )
            //    return _statement.Value;

            FormatSelect();
            FormatInto();
            FormatFrom();
            FormatWhere();
            FormatGroupBy();

            if ( _statement.SetOperation != null )
                FormatSetOperation();
            else
            {
                FormatOrderBy();
                FormatTerminator();
            }
        }
    }
}
