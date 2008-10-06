using System;

namespace Laan.SQL.Parser
{
    /*
          <expression>   ::= <term> [+,-] <expression> | <term>
          <term>         ::= <factor> [*,/,%,^] <term> | <factor>
          <factor>       ::= IDENTIFIER [ ( <expression> ) ] | ( <expression> ) 
     */

    public class ExpressionParser : CustomParser
    {
        public ExpressionParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public Expression Execute()
        {
            return ReadExpression();
        }

        private Expression ReadExpression()
        {
            Expression term = ReadTerm();

            if ( IsNextToken( "+", "-" ) )
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

            if ( IsNextToken( "*", "/", "%", "^" ) )
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
            if ( Tokenizer.TokenEquals( Constants.OPEN_BRACKET ) )
            {
                Expression result = ReadExpression();
                ExpectToken( Constants.CLOSE_BRACKET );
                return new NestedExpression() { Expression = result };
            }
            else
            {

                if ( IsNextToken( "SELECT" ) )
                {
                    ReadNextToken();
                    SelectExpression selectExpression = new SelectExpression();

                    var parser = new SelectStatementParser( Tokenizer );
                    selectExpression.Statement = ( SelectStatement )parser.Execute();
                    return selectExpression;
                }

                string token;
                // check for quoted identifier (string expression) next
                if ( Tokenizer.TokenEquals( Constants.QUOTE ) )
                {
                    token = GetIdentifierUntilTerminated( Constants.QUOTE );
                    ExpectToken( Constants.QUOTE );

                    return new StringExpression( token );
                }

                // get (possibly dot notated) identifier next
                token = GetDotNotationIdentifier();

                // check for an open bracket, indicating that the previous identifier is actually a function
                if ( Tokenizer.TokenEquals( Constants.OPEN_BRACKET ) )
                {
                    FunctionExpression result = new FunctionExpression();
                    result.Name = token;
                    if ( !IsNextToken( Constants.CLOSE_BRACKET ) )
                        do
                        {
                            result.Arguments.Add( ReadExpression() );
                        }
                        while ( Tokenizer.TokenEquals( Constants.COMMA ) );

                    ExpectToken( Constants.CLOSE_BRACKET );
                    return result;
                }
                else
                    return new IdentifierExpression( token );
            }
        }
    }
}
