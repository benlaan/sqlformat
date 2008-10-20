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

            if ( Tokenizer.IsNextToken( "=", ">=", "<=", ">", "<", "IS", "IN", "ANY" ) )
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
            if ( Tokenizer.TokenEquals( Constants.OpenBracket ) )
            {
                Expression result = ReadCriteriaList();
                ExpectToken( Constants.CloseBracket );
                return new NestedExpression() { Expression = result };
            }
            else
            {
                if ( Tokenizer.IsNextToken( "SELECT" ) )
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
                if ( Tokenizer.TokenEquals( Constants.OpenBracket ) )
                {
                    FunctionExpression result = new FunctionExpression();
                    result.Name = token;
                    if ( !Tokenizer.IsNextToken( Constants.CloseBracket ) )
                        do
                        {
                            result.Arguments.Add( ReadExpression() );
                        }
                        while ( Tokenizer.TokenEquals( Constants.Comma ) );

                    ExpectToken( Constants.CloseBracket );
                    return result;
                }
                else
                    return new IdentifierExpression( token );
            }
        }
    }
}
