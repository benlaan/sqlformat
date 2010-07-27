using System;

using Laan.Sql.Parser.Expressions;
using System.Text;

namespace Laan.Sql.Formatter
{
    public class NestedExpressionFormatter : CustomExpressionFormatter<NestedExpression>
    {
        public NestedExpressionFormatter( NestedExpression expression )
            : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            string formattedValue = _expression.Expression.FormattedValue( 0, this ).Replace( "\r\n", " " );
            if ( CanInlineExpression( _expression.Expression, Offset + formattedValue.Length) )
            {
                return FormatBrackets( formattedValue );
            }
            else
            {
                var sql = new StringBuilder( "(" );
                sql.AppendLine();
                sql.AppendLine();
                using ( new IndentScope( this ) )
                {
                    sql.Append( _expression.Expression.FormattedValue( Offset, this ) );
                }
                sql.AppendLine();
                sql.AppendLine();
                sql.Append( GetIndent( false ) + ")" );

                return sql.ToString();
            }
        }

        #endregion
    }
}
