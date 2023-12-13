using System;
using System.Text;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class NestedExpressionFormatter : CustomExpressionFormatter<NestedExpression>
    {
        public NestedExpressionFormatter(NestedExpression expression) : base(expression)
        {
        }

        public override string Execute()
        {
            string formattedValue = _expression.Expression.FormattedValue(0, this).Replace(Environment.NewLine, " ");
            if (CanInlineExpression(_expression.Expression, Offset + formattedValue.Length))
            {
                return BaseFormatter.FormatBrackets(formattedValue);
            }
            else
            {
                var sql = new StringBuilder();
                sql.AppendLine("(");
                sql.AppendLine();
                using (new IndentScope(this))
                {
                    sql.Append(_expression.Expression.FormattedValue(Offset, this));
                }
                sql.AppendLine();
                sql.AppendLine();
                sql.Append(GetIndent(false) + ")");

                return sql.ToString();
            }
        }
    }
}
