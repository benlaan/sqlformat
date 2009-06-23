using System;

using Laan.SQL.Parser.Expressions;

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

        private Expression ReadCriteria( Expression parent )
        {
            Expression expression = ReadExpression( parent );

            if ( Tokenizer.IsNextToken( "=", "<>", "!=", ">=", "<=", ">", "<", "IS", "IN", "ANY", "LIKE" ) )
            {
                CriteriaExpression result = new CriteriaExpression( parent );
                result.Left = expression;

                result.Operator = CurrentToken;
                ReadNextToken();

                result.Right = ReadExpression( parent );

                return result;
            }
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

        private Expression ReadFactor( Expression parent )
        {
            // nested expressions first
            if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
            {
                Expression result;
                using ( Tokenizer.ExpectBrackets() )
                {
                    result = ReadCriteriaList( parent );

                    if ( ( result is IdentifierExpression ) && Tokenizer.IsNextToken( Constants.Comma ) )
                    {
                        var list = new IdentifierListExpression();
                        list.Identifiers.Add( result as IdentifierExpression );
                        
                        do
                        {
                            Tokenizer.ExpectToken( Constants.Comma );

                            result = ReadCriteriaList( parent );
                            list.Identifiers.Add( result as IdentifierExpression );

                        } while ( Tokenizer.IsNextToken( Constants.Comma ) );

                        result = list;
                    }

                }

                NestedExpression nestedExpression = new NestedExpression( parent ) { Expression = result };
                result.Parent = nestedExpression;
                return nestedExpression;
            }
            else
            {
                if ( Tokenizer.IsNextToken( Constants.Case ) )
                {
                    ReadNextToken();
                    if ( Tokenizer.IsNextToken( Constants.When ) )
                        return GetCaseWhenExpression( parent );
                    else
                        return GetCaseSwitchExpression( parent );
                }
                else if ( Tokenizer.IsNextToken( Constants.Select ) )
                {
                    ReadNextToken();
                    SelectExpression selectExpression = new SelectExpression();

                    var parser = new SelectStatementParser( Tokenizer );
                    selectExpression.Statement = ( SelectStatement )parser.Execute();
                    return selectExpression;
                }

                string token;

                if ( Tokenizer.Current.StartsWith( Constants.Quote ) && Tokenizer.Current.EndsWith( Constants.Quote ) )
                {
                    token = Tokenizer.Current;
                    ReadNextToken();
                    return new StringExpression( token, parent );
                }

                if ( Tokenizer.IsNextToken( Constants.Not ) )
                {
                    ReadNextToken();
                    NegationExpression negationExpression = new NegationExpression( parent );
                    var result = ReadCriteriaList( negationExpression );
                    negationExpression.Expression = result;
                    return negationExpression;
                }

                // get (possibly dot notated) identifier next
                token = GetDotNotationIdentifier();

                // check for an open bracket, indicating that the previous identifier is actually a function
                if ( Tokenizer.IsNextToken( Constants.OpenBracket ) )
                {
                    FunctionExpression result = new FunctionExpression( parent );
                    using ( Tokenizer.ExpectBrackets() )
                    {
                        result.Name = token;
                        if ( !Tokenizer.IsNextToken( Constants.CloseBracket ) )
                            do
                            {
                                result.Arguments.Add( ReadExpression( parent ) );
                            }
                            while ( Tokenizer.TokenEquals( Constants.Comma ) );
                    }

                    return result;
                }
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

