using System;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser;

namespace Laan.Sql.Formatter
{
    public class NegationExpressionFormatter : CustomExpressionFormatter<NegationExpression>
    {
        public NegationExpressionFormatter(NegationExpression expression) : base(expression)
        {
        }

        public NegationExpressionFormatter(NegationExpression expression, IIndentable parent) : base(expression, parent)
        {
        }

        public override string Execute()
        {
            return Keyword(Constants.Not) + " " + _expression.Expression.FormattedValue(Offset, this);
        }
    }
}
