using System;
using Laan.SQL.Parser.Expressions;
using System.Text;
using Laan.SQL.Parser;

namespace Laan.SQL.Formatter
{
    public class BetweenExpressionFormatter : CustomExpressionFormatter<BetweenExpression>
    {
        public BetweenExpressionFormatter( BetweenExpression expression ) : base( expression )
        {

        }

        public override string Execute()
        {
            if ( _expression.CanInline )
                return _expression.Value;

            var _sql = new StringBuilder( );
            _sql.AppendFormat( "{0} {1} ", _expression.Expression.FormattedValue( Offset, this ), Constants.Between );

            int offset = Offset + GetCurrentColumn( _sql ) - Constants.And.Length;
            _sql.Append( _expression.From.FormattedValue( Offset, this ) );
            _sql.AppendFormat(
                "\n{0}{1} {2}",
                GetSpaces(offset),
                Constants.And,
                _expression.To.FormattedValue( offset, this)
            );

            return _sql.ToString();
        }
    }
}
