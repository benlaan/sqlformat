using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Parsers
{
    /*
          <criteria>     ::= <expression> [=,<,>,<=,>=,IS] <expression> | <expression>
          <expression>   ::= <term> [+,-] <expression> | <term>
          <term>         ::= <factor> [*,/,%,^] <term> | <factor>
          <factor>       ::= IDENTIFIER [ ( <expression> ) ] | ( <expression> ) 
     */

    public class ExpressionParser : CustomParser
    {
        public ExpressionParser(ITokenizer tokenizer) : base(tokenizer) { }

        public Expression Execute()
        {
            return ReadCriteriaList(null);
        }

        public T Execute<T>() where T : Expression
        {
            return (T)Execute();
        }

        private Expression ReadCriteriaList(Expression parent)
        {
            Expression expression = ReadCriteria(parent);

            if (Tokenizer.IsNextToken("AND", "OR"))
            {
                CriteriaExpression result = new CriteriaExpression(parent);
                result.Left = expression;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadCriteriaList(result);

                return result;
            }
            else
                return expression;
        }

        private BetweenExpression ProcessBetween(Expression parent, Expression expression)
        {
            BetweenExpression betweenExpression = new BetweenExpression(parent);
            betweenExpression.Expression = expression;
            betweenExpression.From = ReadCriteria(betweenExpression);

            ExpectToken(Constants.And);
            betweenExpression.To = ReadCriteria(betweenExpression);

            return betweenExpression;
        }

        private CriteriaExpression ProcessCriteria(Expression parent, Expression expression)
        {
            CriteriaExpression result = new CriteriaExpression(parent);
            result.Left = expression;

            result.Operator = CurrentToken;
            ReadNextToken();

            result.Right = ReadExpression(parent);

            return result;
        }

        private Expression ReadCriteria(Expression parent)
        {
            Expression expression = ReadExpression(parent);

            // this handles the (non-standard) case of NOT being placed after the 'left' operand, instead
            // of the more accurate grammar of being before the operand
            // e.g. WHERE NOT A.ID IN (10, 20) ~ WHERE A.ID NOT IN (10, 20)
            // Although probably inaccurate, it is easiest to simply make the NOT part of the operator
            if (Tokenizer.TokenEquals(Constants.Not))
            {
                if (Tokenizer.TokenEquals(Constants.Between))
                {
                    var between = ProcessBetween(parent, expression);
                    between.Negated = true;
                    return between;
                }

                if (Tokenizer.IsNextToken(Constants.In, Constants.Like))
                {
                    CriteriaExpression criteria = ProcessCriteria(parent, expression);
                    criteria.Operator = Constants.Not + " " + criteria.Operator;
                    return criteria;
                }

                throw new ExpectedTokenNotFoundException("IN or BETWEEN", CurrentToken, Tokenizer.Position);
            }

            if (Tokenizer.TokenEquals(Constants.Between))
                return ProcessBetween(parent, expression);

            if (Tokenizer.IsNextToken("=", "<>", "!=", ">=", "<=", ">", "<", "IS", "IN", "ANY", "LIKE"))
                return ProcessCriteria(parent, expression);

            return expression;
        }

        private Expression ReadExpression(Expression parent)
        {
            Expression term = ReadTerm(parent);

            if (Tokenizer.IsNextToken("+", "-"))
            {
                OperatorExpression result = new OperatorExpression(parent);
                result.Left = term;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression(parent);

                return result;
            }

            return term;
        }

        private Expression ReadTerm(Expression parent)
        {
            Expression factor = ReadFactor(parent);

            if (Tokenizer.IsNextToken(Constants.Over))
            {
                var rankingFunctionExpression = new RankingFunctionExpression(parent);
                rankingFunctionExpression.Name = factor.Value;
                ReadNextToken();
                using (Tokenizer.ExpectBrackets())
                {
                    if (Tokenizer.TokenEquals(Constants.Partition))
                    {
                        ExpectToken(Constants.By);
                        var selectStatementParser = new SelectStatementParser(Tokenizer);
                        selectStatementParser.ProcessFields(FieldType.OrderBy, rankingFunctionExpression.PartitionBy);
                    }

                    if (Tokenizer.IsNextToken(Constants.Order))
                    {
                        ReadNextToken();
                        ExpectToken(Constants.By);
                        var selectStatementParser = new SelectStatementParser(Tokenizer);
                        selectStatementParser.ProcessFields(FieldType.OrderBy, rankingFunctionExpression.OrderBy);
                    }
                }

                return rankingFunctionExpression;
            }
            else if (Tokenizer.IsNextToken("*", "/", "%", "^"))
            {
                OperatorExpression result = new OperatorExpression(parent);
                result.Left = factor;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression(parent);

                return result;
            }

            return factor;
        }

        private SqlType GetProcessType()
        {
            SqlTypeParser sqlTypeParser = new SqlTypeParser(Tokenizer);
            return sqlTypeParser.Execute();
        }

        private Expression GetNestedExpression(Expression parent)
        {
            Expression result;
            using (Tokenizer.ExpectBrackets())
            {
                result = ReadCriteriaList(parent);
                if (Tokenizer.IsNextToken(Constants.Comma))
                {
                    var list = new ExpressionList();
                    list.Identifiers.Add(result);

                    do
                    {
                        Tokenizer.ExpectToken(Constants.Comma);

                        result = ReadCriteriaList(parent);
                        list.Identifiers.Add(result);

                    } while (Tokenizer.IsNextToken(Constants.Comma));

                    result = list;
                }
            }

            NestedExpression nestedExpression = new NestedExpression(parent) { Expression = result };
            result.Parent = nestedExpression;
            return nestedExpression;
        }

        private Expression GetFunction(Expression parent, string token)
        {
            FunctionExpression result = null;
            var arguments = new List<Expression>();
            string functionName = "";

            using (Tokenizer.ExpectBrackets())
            {
                functionName = token;
                if (!Tokenizer.IsNextToken(Constants.CloseBracket))
                    do
                    {
                        if (String.Compare(functionName, Constants.Count, true) == 0)
                        {
                            var distinct = Tokenizer.TokenEquals(Constants.Distinct);
                            result = new CountExpression(parent, distinct);
                        }

                        arguments.Add(ReadExpression(parent));

                        if (Tokenizer.TokenEquals(Constants.As))
                        {
                            if (!String.Equals(functionName, Constants.Cast, StringComparison.InvariantCultureIgnoreCase))
                                throw new SyntaxException("AS is allowed only within a CAST expression");

                            result = new CastExpression(parent, GetProcessType());
                            break;
                        }
                    }
                    while (Tokenizer.TokenEquals(Constants.Comma));
            }

            result = result ?? new FunctionExpression(parent) { Name = functionName };
            result.Arguments = arguments;
            return result;
        }

        private Expression GetCaseExpression(Expression parent)
        {
            ReadNextToken();
            if (Tokenizer.IsNextToken(Constants.When))
                return GetCaseWhenExpression(parent);
            
            return GetCaseSwitchExpression(parent);
        }

        private Expression GetSelectExpression()
        {
            ReadNextToken();
            SelectExpression selectExpression = new SelectExpression();

            var parser = new SelectStatementParser(Tokenizer);
            selectExpression.Statement = (SelectStatement)parser.Execute();
            return selectExpression;
        }

        private StringExpression ReadString(Expression parent)
        {
            var value = Tokenizer.Current.Value;
            ReadNextToken();
            while (Tokenizer.Current != (Token)null && Tokenizer.Current.Type == TokenType.String)
            {
                value += Tokenizer.Current.Value;
                ReadNextToken();
            };

            return new StringExpression(value, parent);
        }

        private Expression ReadFactor(Expression parent)
        {
            // nested expressions first
            if (Tokenizer.IsNextToken(Constants.OpenBracket))
            {
                return GetNestedExpression(parent);
            }
            else
            {
                if (Tokenizer.IsNextToken(Constants.Case))
                    return GetCaseExpression(parent);

                if (Tokenizer.IsNextToken(Constants.Select))
                    return GetSelectExpression();

                if (Tokenizer.HasMoreTokens && Tokenizer.Current.Type == TokenType.String)
                {
                    return ReadString(parent);
                }

                if (Tokenizer.IsNextToken(Constants.Not))
                {
                    ReadNextToken();
                    NegationExpression negationExpression = new NegationExpression(parent);
                    var result = ReadCriteriaList(negationExpression);
                    negationExpression.Expression = result;
                    return negationExpression;
                }

                if (Tokenizer.Current != (Token)null && !Tokenizer.Current.IsTypeIn(
                    TokenType.SingleQuote, TokenType.Variable, TokenType.AlphaNumeric, TokenType.AlphaNumeric,
                    TokenType.Numeric, TokenType.Operator, TokenType.BlockedText)
                )
                    throw new SyntaxException("expected alpha, numeric, or variable, found " + Tokenizer.Current.Value);

                // get (possibly dot notated) identifier next
                string token = GetDotNotationIdentifier();

                // check for an open bracket, indicating that the previous identifier is actually a function
                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                    return GetFunction(parent, token);
                else
                    return new IdentifierExpression(token, parent);
            }
        }

        private void ParseCaseExpression(CaseExpression caseExpression)
        {
            while (Tokenizer.IsNextToken(Constants.When))
            {
                ReadNextToken();
                CaseSwitch caseSwitch = new CaseSwitch(caseExpression);
                caseSwitch.When = ReadCriteriaList(caseExpression);

                Tokenizer.ExpectToken(Constants.Then);
                caseSwitch.Then = ReadExpression(caseExpression);

                caseExpression.Cases.Add(caseSwitch);
            }

            if (Tokenizer.IsNextToken(Constants.Else))
            {
                ReadNextToken();
                caseExpression.Else = ReadExpression(caseExpression);
            }

            ExpectToken(Constants.End);
        }

        private Expression GetCaseWhenExpression(Expression parent)
        {
            CaseWhenExpression result = new CaseWhenExpression(parent);
            ParseCaseExpression(result);
            return result;
        }

        private Expression GetCaseSwitchExpression(Expression parent)
        {
            CaseSwitchExpression result = new CaseSwitchExpression(parent);
            result.Switch = ReadCriteriaList(result);
            ParseCaseExpression(result);
            return result;
        }
    }
}
