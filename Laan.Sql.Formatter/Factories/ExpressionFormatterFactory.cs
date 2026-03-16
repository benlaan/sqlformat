using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Formatter
{
    public class ExpressionFormatterFactory
    {
        public static IExpressionFormatter GetFormatter(IIndentable indentable, Expression expression)
        {
            IExpressionFormatter formatter = null;
            switch (expression)
            {
                case CriteriaExpression criteriaExpression:
                    formatter = new CriteriaExpressionFormatter(criteriaExpression);
                    break;

                case CaseSwitchExpression caseSwitchExpression:
                    formatter = new CaseSwitchExpressionFormatter(caseSwitchExpression);
                    break;

                case CaseWhenExpression caseWhenExpression:
                    formatter = new CaseWhenExpressionFormatter(caseWhenExpression);
                    break;

                case FunctionExpression functionExpression:
                    formatter = new FunctionExpressionFormatter(functionExpression);
                    break;

                case NestedExpression nestedExpression:
                    formatter = new NestedExpressionFormatter(nestedExpression);
                    break;

                case SelectExpression selectExpression:
                    formatter = new SelectExpressionFormatter(selectExpression);
                    break;

                case ExpressionList expressionList:
                    formatter = new ExpressionListFormatter(expressionList);
                    break;

                case BetweenExpression betweenExpression:
                    formatter = new BetweenExpressionFormatter(betweenExpression);
                    break;

                case NegationExpression negationExpression:
                    formatter = new NegationExpressionFormatter(negationExpression);
                    break;

                default:
                    formatter = new DefaultExpressionFormatter(expression);
                    break;
            }

            var indentableFormatter = formatter as IIndentable;
            if (indentableFormatter != null)
            {
                indentableFormatter.IndentLevel = indentable.IndentLevel;
                indentableFormatter.Indent = indentable.Indent;
            }

            return formatter;
        }
    }
}
