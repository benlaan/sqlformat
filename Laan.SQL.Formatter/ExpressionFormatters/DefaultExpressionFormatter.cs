using System;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Formatter
{
    public class DefaultExpressionFormatter : CustomExpressionFormatter<Expression>
    {
        public DefaultExpressionFormatter( Expression expression )
            : base( expression )
        {

        }

        public override string Execute()
        {
            return _expression.Value;
        }
    }
}
