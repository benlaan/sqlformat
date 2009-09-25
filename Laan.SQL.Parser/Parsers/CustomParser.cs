using System;
using System.Collections.Generic;

using Laan.SQL.Parser.Expressions;

namespace Laan.SQL.Parser
{
    public abstract class CustomParser
    {
        public CustomParser( ITokenizer tokenizer )
        {
            Tokenizer = tokenizer;
        }

        protected void ExpectToken( string token )
        {
            if ( CurrentToken.ToLower() != token.ToLower() )
                throw new ExpectedTokenNotFoundException( token, CurrentToken, Tokenizer.Position );
            else
                ReadNextToken();
        }

        protected void ExpectTokens( params string[] tokens )
        {
            foreach ( string token in tokens )
                ExpectToken( token );
        }

        protected void ReadNextToken()
        {
            Tokenizer.ReadNextToken();
        }

        protected string CurrentToken
        {
            get { return Tokenizer.Current.Value; }
        }

        protected ITokenizer Tokenizer { get; private set; }

        protected Expression ProcessExpression()
        {
            var parser = new ExpressionParser( Tokenizer );
            return parser.Execute();
        }

        private string GetOperator()
        {
            if ( Tokenizer.IsNextToken( "=", ">=", "<=", "!=", "<>", "IN", "ANY", "LIKE" ) )
            {
                string token = Tokenizer.Current.Value;
                ReadNextToken();
                return token;
            }
            else
                throw new ExpectedTokenNotFoundException(
                    "'=', '>=', '<=', '!=', '<>', 'IN', 'ANY', 'LIKE'",
                    CurrentToken,
                    Tokenizer.Position
                );
        }

        protected CriteriaExpression ProcessCriteriaExpression( Expression parent )
        {
            CriteriaExpression expression = new CriteriaExpression( parent );
            expression.Left = ProcessExpression();
            expression.Operator = GetOperator();
            expression.Right = ProcessExpression();

            return expression;
        }

        protected string GetIdentifier()
        {
            if ( !Tokenizer.HasMoreTokens )
                throw new SyntaxException( "Identifier expected" );

            string identifier = CurrentToken;
            ReadNextToken();
            return identifier;
        }

        protected List<string> GetIdentifierList()
        {
            List<string> identifiers = new List<string>();
            do
            {
                identifiers.Add( GetIdentifier() );
            }
            while ( Tokenizer.TokenEquals( Constants.Comma ) );

            return identifiers;
        }

        protected string GetDotNotationIdentifier()
        {
            string token;
            token = "";

            do
            {
                token += ( token != "" ? Constants.Dot : "" ) + GetIdentifier();
            }
            while ( Tokenizer.TokenEquals( Constants.Dot ) );
            return token;
        }

        protected bool HasTerminator()
        {
            bool result = Tokenizer.IsNextToken( Constants.SemiColon );
            if ( result )
                Tokenizer.ReadNextToken();
            return result;
        }
    }
}
