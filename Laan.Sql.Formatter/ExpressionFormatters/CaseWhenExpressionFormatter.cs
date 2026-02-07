using System;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class CaseWhenExpressionFormatter : CaseExpressionFormatter<CaseWhenExpression>
    {
        public CaseWhenExpressionFormatter( CaseWhenExpression expression ) : base( expression )
        {
        }

        public CaseWhenExpressionFormatter( CaseWhenExpression expression, IIndentable parent ) : base( expression, parent )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( CanInlineExpression( _expression, Offset ) )
                return _expression.Value;

            var sql = new StringBuilder();

            sql.AppendFormat( "{0}{1}", _expression.Parent is CaseExpression ? GetIndent( true ) : "", Keyword(Constants.Case) );
            foreach ( var caseItem in _expression.Cases )
            {
                using ( new IndentScope( this ) )
                {
                    sql.AppendFormat( "{0}{1} ", GetIndent( true ), Keyword(Constants.When) );
                    sql.AppendFormat( "{0} {1} ", caseItem.When.FormattedValue( Offset, this ), Keyword(Constants.Then) );

                    int off = GetCurrentColumn( sql );
                    using ( new IndentScope( this ) )
                        sql.Append( caseItem.Then.FormattedValue( Offset + off, this ) );
                }
            }

            if ( _expression.Else != null )
                sql.Append( FormatCaseElseExpression( Offset, _expression ) );

            sql.Append( GetIndent( true ) + Keyword(Constants.End) );

            return sql.ToString();
        }

        #endregion
    }
}
