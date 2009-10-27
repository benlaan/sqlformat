using System;

using Laan.SQL.Parser.Expressions;
using System.Text;

namespace Laan.SQL.Formatter
{
    public class NestedExpressionFormatter : CustomExpressionFormatter<NestedExpression>
    {
        public NestedExpressionFormatter( NestedExpression expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( CanInlineExpression( _expression.Expression, Offset ) )
                return _expression.Value;
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

                //sql.AppendLine();
                //sql.AppendLine();
                //sql.Append( GetIndent( Indent, IndentLevel + 1, false ) + _expression.Expression.FormattedValue( Offset, this ) );
                //sql.AppendLine( GetIndent( Indent, IndentLevel - 1 ) );
                //sql.Append( GetIndent( Indent, IndentLevel, false ) + ")" );
                return sql.ToString();
            }
        }

        #endregion
    }
}
