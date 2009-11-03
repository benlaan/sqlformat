using System;
using System.Text;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public abstract class CaseExpressionFormatter<T> : CustomExpressionFormatter<T> where T : CaseExpression
    {
        public CaseExpressionFormatter( T expression )
            : base( expression )
        {
        }

        protected string FormatCaseElseExpression( int offset, CaseExpression caseSwitch )
        {
            StringBuilder sql = new StringBuilder( GetIndent( true ) + "ELSE" );
            using ( new IndentScope( this ) )
            {
                sql.Append( GetIndent( true ) + caseSwitch.Else.FormattedValue( offset, this ) );
            }

            return sql.ToString();
        }
    }

    public class CaseSwitchExpressionFormatter : CaseExpressionFormatter<CaseSwitchExpression>
    {
        public CaseSwitchExpressionFormatter( CaseSwitchExpression expression )
            : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            if ( CanInlineExpression( _expression, Offset ) )
                return _expression.Value;

            var caseSwitch = (CaseSwitchExpression) _expression;
            bool isNested = IndentLevel > 1;

            var sql = new StringBuilder(
                String.Format(
                    "{0}CASE {1}",
                    isNested ? GetIndent( true ) : "",
                    caseSwitch.Switch.FormattedValue( Offset, this )
                )
            );

            using ( new IndentScope( this ) )
            {
                foreach ( var caseItem in caseSwitch.Cases )
                {
                    sql.AppendFormat(
                        "{0}WHEN {1} THEN ",
                        GetIndent( true ),
                        caseItem.When.FormattedValue( Offset, this )
                    );
                    using ( new IndentScope( this ) )
                        sql.Append( caseItem.Then.FormattedValue( Offset, this ) );
                }
            }

            if ( caseSwitch.Else != null )
                sql.Append( FormatCaseElseExpression( Offset, caseSwitch ) );

            sql.Append( GetIndent( true ) + "END" );

            return sql.ToString();
        }

        #endregion
    }
}
