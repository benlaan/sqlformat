using System;
using System.Text;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class BetweenExpressionFormatter : CustomExpressionFormatter<BetweenExpression>
    {
        public BetweenExpressionFormatter( BetweenExpression expression ) : base( expression )
        {
        }

        public override string Execute()
        {
            if ( _expression.CanInline )
                return String.Format(
                    "{0} {1}BETWEEN {2} AND {3}",
                    _expression.Expression.FormattedValue( Offset, this ),
                    _expression.Negated ? "NOT " : "",
                    _expression.From.FormattedValue( Offset, this ),
                    _expression.To.FormattedValue( Offset, this )
                );

            var _sql = new StringBuilder();
            _sql.AppendFormat( "{0} {1} ", _expression.Expression.FormattedValue( Offset, this ), Constants.Between );

            int offset = Offset + GetCurrentColumn( _sql ) - Constants.And.Length;
            _sql.Append( _expression.From.FormattedValue( Offset, this ) );
            _sql.AppendFormat(
                "\n{0}{1} {2}",
                GetSpaces( offset ),
                Constants.And,
                _expression.To.FormattedValue( offset, this )
            );

            return _sql.ToString();
        }
    }
}
