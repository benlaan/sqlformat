using System;

namespace Laan.SQL.Parser
{
    /*
          <criteria>     ::= <expression> [=,<,>,<=,>=] <expression> | <expression>
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

            if ( IsNextToken( "AND", "OR" ) )
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

            if ( IsNextToken( "=", ">=", "<=", ">", "<", "IN", "ANY" ) )
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
                Expression result = ReadCriteriaList();
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

                if ( Tokenizer.Current.StartsWith( "'" ) && Tokenizer.Current.EndsWith( "'" ) )
                {
                    token = Tokenizer.Current;
                    ReadNextToken();
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
