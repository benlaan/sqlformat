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
                throw new ExpectedTokenNotFoundException( token, CurrentToken );
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

        protected bool IsNextToken( params string[] tokenSet )
        {
            foreach ( var token in tokenSet )
                if ( token.ToLower() == CurrentToken.ToLower() )
                    return true;

            return false;
        }

        protected Tokenizer Tokenizer { get; private set; }

        protected Expression ProcessExpression()
        {
            var parser = new ExpressionParser( Tokenizer );
            return parser.Execute();
        }

        private string GetOperator()
        {
            if ( IsNextToken( "=", ">=", "<=", "!=", "<>", "IN", "ANY" ) )
            {
                string token = Tokenizer.Current;
                ReadNextToken();
                return token;
            }
            else
                throw new ExpectedTokenNotFoundException( "'=', '>=', '<=', '!=', '<>', 'IN', 'ANY'", CurrentToken );

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
                while ( !IsNextToken( terminator ) );

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
            if ( Tokenizer.TokenEquals( Constants.OPEN_SQUARE_BRACE ) )
            {
                identifier += Constants.OPEN_SQUARE_BRACE + GetIdentifierUntilTerminated(Constants.CLOSE_SQUARE_BRACE) + Constants.CLOSE_SQUARE_BRACE;
                Tokenizer.ExpectToken( Constants.CLOSE_SQUARE_BRACE );
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
            while ( Tokenizer.TokenEquals( Constants.COMMA ) );

            return identifiers;
        }
        
        protected string GetDotNotationIdentifier()
        {
            string token;
            token = "";
            do
            {
                token += ( token != "" ? Constants.DOT : "" ) + GetIdentifier();
            }
            while ( Tokenizer.TokenEquals( Constants.DOT ) );
            return token;
        }
    }
}
