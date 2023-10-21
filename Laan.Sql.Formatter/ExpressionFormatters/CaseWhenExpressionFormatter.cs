using System;
using System.Text;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class CaseWhenExpressionFormatter : CaseExpressionFormatter<CaseWhenExpression>
    {
        public CaseWhenExpressionFormatter( CaseWhenExpression expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( CanInlineExpression( _expression, Offset ) )
                return _expression.Value;

            var sql = new StringBuilder();

            sql.AppendFormat( "{0}CASE", _expression.Parent is CaseExpression ? GetIndent( true ) : "" );
            foreach ( var caseItem in _expression.Cases )
            {
                using ( new IndentScope( this ) )
                {
                    sql.AppendFormat( "{0}WHEN ", GetIndent( true ) );
                    sql.AppendFormat( "{0} THEN ", caseItem.When.FormattedValue( Offset, this ) );

                    int off = GetCurrentColumn( sql );
                    using ( new IndentScope( this ) )
                        sql.Append( caseItem.Then.FormattedValue( Offset + off, this ) );
                }
            }

            if ( _expression.Else != null )
                sql.Append( FormatCaseElseExpression( Offset, _expression ) );

            sql.Append( GetIndent( true ) + "END" );

            return sql.ToString();
        }

        #endregion
    }
}
