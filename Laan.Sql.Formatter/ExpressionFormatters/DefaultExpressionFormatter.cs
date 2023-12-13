using System;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
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
