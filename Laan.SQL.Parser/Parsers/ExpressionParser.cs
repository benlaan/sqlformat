using System;

using Laan.Sql.Parser.Expressions;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using System.Collections.Generic;
using System.Net;

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
        public ExpressionParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public Expression Execute()
        {
            return ReadCriteriaList( null );
        }

        private Expression ReadCriteriaList( Expression parent )
        {
            Expression expression = ReadCriteria( parent );

            if ( Tokenizer.IsNextToken( "AND", "OR" ) )
            {
                CriteriaExpression result = new CriteriaExpression( parent );
                result.Left = expression;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadCriteriaList( result );

                return result;
            }
            else
                return expression;
        }

        private BetweenExpression ProcessBetween( Expression parent, Expression expression )
        {
            BetweenExpression betweenExpression = new BetweenExpression( parent );
            betweenExpression.Expression = expression;
            betweenExpression.From = ReadCriteria( betweenExpression );

            ExpectToken( Constants.And );
            betweenExpression.To = ReadCriteria( betweenExpression );

            return betweenExpression;
        }

        private CriteriaExpression ProcessCriteria( Expression parent, Expression expression )
        {
            CriteriaExpression result = new CriteriaExpression( parent );
            result.Left = expression;

            result.Operator = CurrentToken;
            ReadNextToken();

            result.Right = ReadExpression( parent );

            return result;
        }

        private Expression ReadCriteria( Expression parent )
        {
            Expression expression = ReadExpression( parent );

            // this handles the (non-standard) case of NOT being placed after the 'left' operand, instead
            // of the more accurate grammar of being before the operand
            // e.g. WHERE NOT A.ID IN (10, 20) ~ WHERE A.ID NOT IN (10, 20)
            // Although probably inaccurate, it is easiest to simply make the NOT part of the operator
            if ( Tokenizer.TokenEquals( Constants.Not ) )
            {
                if ( Tokenizer.TokenEquals( Constants.Between ) )
                {
                    var between = ProcessBetween( parent, expression );
                    between.Negated = true;
                    return between;
                }

                if ( Tokenizer.IsNextToken( Constants.In, Constants.Like ) )
                {
                    CriteriaExpression criteria = ProcessCriteria( parent, expression );
                    criteria.Operator = Constants.Not + " " + criteria.Operator;
                    return criteria;
                }

                throw new ExpectedTokenNotFoundException( "IN or BETWEEN", CurrentToken, Tokenizer.Position );
            }
            else 
            if ( Tokenizer.TokenEquals( Constants.Between ) )
                return ProcessBetween( parent, expression );
            else 
            if ( Tokenizer.IsNextToken( "=", "<>", "!=", ">=", "<=", ">", "<", "IS", "IN", "ANY", "LIKE" ) )
                return ProcessCriteria( parent, expression );
            else
                return expression;
        }

        private Expression ReadExpression( Expression parent )
        {
            Expression term = ReadTerm( parent );

            if ( Tokenizer.IsNextToken( "+", "-" ) )
            {
                OperatorExpression result = new OperatorExpression( parent );
                result.Left = term;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression( parent );

                return result;
            }
            else
                return term;
        }

        private Expression ReadTerm( Expression parent )
        {
            Expression factor = ReadFactor( parent );

            if ( Tokenizer.IsNextToken( "*", "/", "%", "^" ) )
            {
                OperatorExpression result = new OperatorExpression( parent );
                result.Left = factor;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression( parent );

                return result;
            }
            else
                return factor;
        }

        private SqlType ProcessType()
        {
            SqlTypeParser sqlTypeParser = new SqlTypeParser(Tokenizer);
            return sqlTypeParser.Execute();
        }

        private Expression GetNestedExpression( Expression parent )
        {
            Expression result;
            using ( Tokenizer.ExpectBrackets() )
            {
                result = ReadCriteriaList( parent );
                if ( Tokenizer.IsNextToken( Constants.Comma ) )
                {
                    var list = new ExpressionList();
                    list.Identifiers.Add( result );

                    do
                    {
                        Tokenizer.ExpectToken( Constants.Comma );

                        result = ReadCriteriaList( parent );
                        list.Identifiers.Add( result );

                    } while ( Tokenizer.IsNextToken( Constants.Comma ) );

                    result = list;
                }
            }

            NestedExpression nestedExpression = new NestedExpression( parent ) { Expression = result };
            result.Parent = nestedExpression;
            return nestedExpression;
        }

        private Expression GetFunction( Expression parent, string token )
        {
            FunctionExpression result = null;
            var arguments = new List<Expression>();
            string functionName = "";

            using ( Tokenizer.ExpectBrackets() )
            {
                functionName = token;
                if ( !Tokenizer.IsNextToken( Constants.CloseBracket ) )
                    do
                    {
                        arguments.Add( ReadExpression( parent ) );
                        if ( Tokenizer.TokenEquals( Constants.As ) )
                        {
                            if ( functionName != Constants.Cast )
                                throw new SyntaxException( "AS is allowed only within a CAST expression" );

                            result = new CastExpression( parent, ProcessType() );
                            break;
                        }
                    }
                    while ( Tokenizer.TokenEquals( Constants.Comma ) );
            }

            result = result ?? new FunctionExpression( parent ) { Name = functionName };
            result.Arguments = arguments;
            return result;
        }


        private Expression GetCaseExpression(Expression parent)
        {
            ReadNextToken();
            if (Tokenizer.IsNextToken(Constants.When))
                return GetCaseWhenExpression(parent);
            else
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

        private Expression ReadFactor(Expression parent)
        {
            // nested expressions first
            if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
            {
                return GetNestedExpression( parent );
            }
            else
            {
                if ( Tokenizer.IsNextToken( Constants.Case ) )
                {
                    return GetCaseExpression(parent);
                }
                else if ( Tokenizer.IsNextToken( Constants.Select ) )
                {
                    return GetSelectExpression();
                }

                string token;
                if ( Tokenizer.HasMoreTokens && Tokenizer.Current.Type == TokenType.SingleQuote ) // StartsWith( Constants.Quote ) && Tokenizer.Current.EndsWith( Constants.Quote ) )
                {
                    var value = Tokenizer.Current.Value;
                    do
                    {
                        ReadNextToken();
                    	value += Tokenizer.Current.Value;
                    } 
                    while (Tokenizer.Current.Type != TokenType.SingleQuote);
                    ReadNextToken();

                    return new StringExpression( value, parent );
                }

                if ( Tokenizer.IsNextToken( Constants.Not ) )
                {
                    ReadNextToken();
                    NegationExpression negationExpression = new NegationExpression( parent );
                    var result = ReadCriteriaList( negationExpression );
                    negationExpression.Expression = result;
                    return negationExpression;
                }

                if ( Tokenizer.Current != (Token) null && !Tokenizer.Current.IsTypeIn(
                    TokenType.SingleQuote, TokenType.Variable, TokenType.Alpha, TokenType.AlphaNumeric,
                    TokenType.Numeric, TokenType.Operator, TokenType.BlockedText )
                )
                    throw new SyntaxException( "expected alpha, numeric, or variable, found " + Tokenizer.Current.Value );

                // get (possibly dot notated) identifier next
                token = GetDotNotationIdentifier();

                // check for an open bracket, indicating that the previous identifier is actually a function
                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                    return GetFunction(parent, token);
                else
                    return new IdentifierExpression( token, parent );
            }
        }

        private void ParseCaseExpression( CaseExpression caseExpression )
        {
            while ( Tokenizer.IsNextToken( Constants.When ) )
            {
                ReadNextToken();
                CaseSwitch caseSwitch = new CaseSwitch( caseExpression );
                caseSwitch.When = ReadCriteriaList( caseExpression );

                Tokenizer.ExpectToken( Constants.Then );
                caseSwitch.Then = ReadExpression( caseExpression );

                caseExpression.Cases.Add( caseSwitch );
            }

            if ( Tokenizer.IsNextToken( Constants.Else ) )
            {
                ReadNextToken();
                caseExpression.Else = ReadExpression( caseExpression );
            }

            ExpectToken( Constants.End );
        }

        private Expression GetCaseWhenExpression( Expression parent )
        {
            CaseWhenExpression result = new CaseWhenExpression( parent );
            ParseCaseExpression( result );
            return result;
        }

        private Expression GetCaseSwitchExpression( Expression parent )
        {
            CaseSwitchExpression result = new CaseSwitchExpression( parent );
            result.Switch = ReadCriteriaList( result );
            ParseCaseExpression( result );
            return result;
        }
    }
}

