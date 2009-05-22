using System;
using System.Collections.Generic;

namespace Laan.SQL.Parser
{
    public abstract class CustomParser
    {
        public CustomParser( Tokenizer tokenizer )
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
            get { return Tokenizer.Current; }
        }

        protected Tokenizer Tokenizer { get; private set; }

        protected Expression ProcessExpression()
        {
            var parser = new ExpressionParser( Tokenizer );
            return parser.Execute();
        }

        private string GetOperator()
        {
            if ( Tokenizer.IsNextToken( "=", ">=", "<=", "!=", "<>", "IN", "ANY", "LIKE" ) )
            {
                string token = Tokenizer.Current;
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

        protected CriteriaExpression ProcessCriteriaExpression()
        {
            CriteriaExpression expression = new CriteriaExpression();
            expression.Left = ProcessExpression();
            expression.Operator = GetOperator();
            expression.Right = ProcessExpression();

            return expression;
        }

        protected string GetIdentifierUntilTerminated( string terminator )
        {
            Tokenizer.AcceptSpaces = true;
            string token = "";
            try
            {
                do
                {
                    token += CurrentToken;
                    ReadNextToken();
                }
                while ( !Tokenizer.IsNextToken( terminator ) );

            }
            finally
            {
                Tokenizer.AcceptSpaces = false;
            }
            return token;
        }

        protected string GetIdentifier()
        {
            string identifier = "";
            if ( Tokenizer.TokenEquals( Constants.OpenSquareBracket ) )
            {
                identifier += Constants.OpenSquareBracket + GetIdentifierUntilTerminated(Constants.CloseSquareBracket) + Constants.CloseSquareBracket;
                Tokenizer.ExpectToken( Constants.CloseSquareBracket );
            }
            else
            {
                identifier = CurrentToken;
                ReadNextToken();
            }
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
    }
}
