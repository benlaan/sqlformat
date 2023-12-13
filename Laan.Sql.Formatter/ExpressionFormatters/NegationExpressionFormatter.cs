using System;

using Laan.Sql.Parser.Expressions;
using System.Text;

namespace Laan.Sql.Formatter
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
