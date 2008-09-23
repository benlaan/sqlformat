using System;

namespace Laan.SQL.Parser
{
    public class ExpressionParser : CustomParser
    {
        public ExpressionParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public Expression Execute()
        {
            Expression expression = null;

            expression = new Expression();
            expression.Value = ReadExpression();

            return expression;
        }

        private string ReadExpression()
        {
            if ( Tokenizer.TokenEquals( Constants.OPEN_BRACKET ) )
            {
                string result = ReadExpression();
                ExpectToken( Constants.CLOSE_BRACKET );
                return result;
            }
            else
            {
                string token = "";
                if ( Tokenizer.TokenEquals( Constants.QUOTE ) )
                {
                    token += Constants.QUOTE + GetIdentifierUntilTerminated( Constants.QUOTE ) + Constants.QUOTE;
                    ExpectToken( Constants.QUOTE );
                }
                else
                {
                    token = GetDotNotationIdentifier();
                    // test whether the current token is a function - ie. SomeToken()
                    if ( Tokenizer.TokenEquals( Constants.OPEN_BRACKET ) )
                    {
                        // TODO: This needs to be implemented as a param list of expressions
                        token += "()";
                        ExpectToken( Constants.CLOSE_BRACKET );
                    }
                }
                return token;
            }
        }

    }
}
