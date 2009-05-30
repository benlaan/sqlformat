using System;

namespace Laan.SQL.Parser
{
    /*
          <criteria>     ::= <expression> [=,<,>,<=,>=,IS] <expression> | <expression>
          <expression>   ::= <term> [+,-] <expression> | <term>
          <term>         ::= <factor> [*,/,%,^] <term> | <factor>
          <factor>       ::= IDENTIFIER [ ( <expression> ) ] | ( <expression> ) 
     */

    public class ExpressionParser : CustomParser
    {
        public ExpressionParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public Expression Execute()
        {
            return ReadCriteriaList();
        }

        private Expression ReadCriteriaList()
        {
            Expression expression = ReadCriteria();

            if ( Tokenizer.IsNextToken( "AND", "OR" ) )
            {
                CriteriaExpression result = new CriteriaExpression();
                result.Left = expression;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadCriteriaList();

                return result;
            }
            else
                return expression;
        }

        private Expression ReadCriteria()
        {
            Expression expression = ReadExpression();

            if ( Tokenizer.IsNextToken( "=", "<>", "!=", ">=", "<=", ">", "<", "IS", "IN", "ANY", "LIKE" ) )
            {
                CriteriaExpression result = new CriteriaExpression();
                result.Left = expression;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression();

                return result;
            }
            else
                return expression;
        }


        private Expression ReadExpression()
        {
            Expression term = ReadTerm();

            if ( Tokenizer.IsNextToken( "+", "-" ) )
            {
                OperatorExpression result = new OperatorExpression();
                result.Left = term;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression();

                return result;
            }
            else
                return term;
        }

        private Expression ReadTerm()
        {
            Expression factor = ReadFactor();

            if ( Tokenizer.IsNextToken( "*", "/", "%", "^" ) )
            {
                OperatorExpression result = new OperatorExpression();
                result.Left = factor;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression();

                return result;
            }
            else
                return factor;
        }

        private Expression ReadFactor()
        {
            // nested expressions first
            if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
            {
                Expression result;
                using ( Tokenizer.ExpectBrackets() )
                {
                    result = ReadCriteriaList();
                }

                return new NestedExpression() { Expression = result };
            }
            else
            {
                if ( Tokenizer.IsNextToken( Constants.Case ) )
                {
                    ReadNextToken();
                    if ( Tokenizer.IsNextToken( Constants.When ) )
                        return GetCaseWhenExpression();
                    else
                        return GetCaseSwitchExpression();
                }
                else if ( Tokenizer.IsNextToken( Constants.Select ) )
                {
                    ReadNextToken();
                    SelectExpression selectExpression = new SelectExpression();

                    var parser = new SelectStatementParser( Tokenizer );
                    selectExpression.Statement = (SelectStatement) parser.Execute();
                    return selectExpression;
                }

                string token;

                if ( Tokenizer.Current.StartsWith( Constants.Quote ) && Tokenizer.Current.EndsWith( Constants.Quote ) )
                {
                    token = Tokenizer.Current;
                    ReadNextToken();
                    return new StringExpression( token );
                }

                if ( Tokenizer.IsNextToken( Constants.Not ) )
                {
                    ReadNextToken();
                    var result = ReadCriteriaList();
                    return new NegationExpression() { Expression = result };
                }

                // get (possibly dot notated) identifier next
                token = GetDotNotationIdentifier();

                // check for an open bracket, indicating that the previous identifier is actually a function
                if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
                {
                    FunctionExpression result = new FunctionExpression();
                    using ( Tokenizer.ExpectBrackets() )
                    {
                        result.Name = token;
                        if ( !Tokenizer.IsNextToken( Constants.CloseBracket ) )
                            do
                            {
                                result.Arguments.Add( ReadExpression() );
                            }
                            while ( Tokenizer.TokenEquals( Constants.Comma ) );
                    }

                    return result;
                }
                else
                    return new IdentifierExpression( token );
            }
        }

        private void ParseCaseExpression( CaseExpression caseExpression )
        {
            while ( Tokenizer.IsNextToken( Constants.When ) )
            {
                ReadNextToken();
                CaseSwitch caseSwitch = new CaseSwitch();
                caseSwitch.When = ReadCriteriaList();

                Tokenizer.ExpectToken( Constants.Then );
                caseSwitch.Then = ReadExpression();

                caseExpression.Cases.Add( caseSwitch );
            }

            if ( Tokenizer.IsNextToken( Constants.Else ) )
            {
                ReadNextToken();
                caseExpression.Else = ReadExpression();
            }

            ExpectToken( Constants.End );
        }

        private Expression GetCaseWhenExpression()
        {
            CaseWhenExpression result = new CaseWhenExpression();
            ParseCaseExpression( result );
            return result;
        }

        private Expression GetCaseSwitchExpression()
        {
            CaseSwitchExpression result = new CaseSwitchExpression();
            result.Switch = ReadCriteriaList();
            ParseCaseExpression( result );
            return result;
        }
    }
}

