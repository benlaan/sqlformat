using System;
using System.Linq;

using Laan.SQL.Parser.Expressions;
using Laan.SQL.Parser;
using System.Text;

namespace Laan.SQL.Formatter
{
    public class FunctionExpressionFormatter : CustomExpressionFormatter<FunctionExpression>
    {
        public FunctionExpressionFormatter( FunctionExpression expression )
            : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( String.Compare( _expression.Name, Constants.Exists, true ) == 0 )
            {
                StringBuilder sql = new StringBuilder();
                sql.AppendFormat( "{0}(\r\n", _expression.Name );
                sql.AppendLine();
                using ( new IndentScope( this ) )
                {
                    sql.Append( _expression.Arguments.First().FormattedValue( Offset, this ) );
                }
                sql.AppendLine();
                sql.Append( GetIndent( true ) + ")" );
                return sql.ToString();
            }
            else
            {
                string[] args = _expression.Arguments
                    .Select( arg => arg.FormattedValue( Offset, this ) )
                    .ToArray();

                bool canInline = _expression.Value.Length <= 40;
                string comma = Constants.Comma + ( canInline ? " " : "" );

                StringBuilder sql = new StringBuilder();
                sql.AppendFormat( "{0}(", _expression.Name );
                using ( new IndentScope( this ) )
                {
                    if ( !canInline )
                        sql.Append( GetIndent( true ) );
                    string separator = !canInline ? comma + GetIndent( true ) : comma;
                    sql.Append( String.Join( separator, args ) );
                }
                sql.Append( ( canInline ? "" : GetIndent( true ) ) + ")" );
                return sql.ToString();
            }

        }

        #endregion
    }
}
