using System;

using Laan.SQL.Parser.Expressions;
using System.Text;

namespace Laan.SQL.Formatter
{
    public class NegationExpressionFormatter : CustomExpressionFormatter<NegationExpression>
    {
        public NegationExpressionFormatter( NegationExpression expression ) : base( expression )
        {
        }

        #region IExpressionFormatter Members

        public override string Execute()
        {
            return "NOT " + _expression.Expression.FormattedValue( Offset, this );
        }

        #endregion
    }
}
