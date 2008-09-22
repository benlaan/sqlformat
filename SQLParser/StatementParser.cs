using System;

namespace Laan.SQL.Parser
{
    /// <summary>
    /// Base class for parsing an SQL statement
    /// </summary>
    public abstract class StatementParser
    {
        protected const string COMMA = ",";
        protected const string OPEN_BRACKET = "(";
        protected const string CLOSE_BRACKET = ")";
        protected const string OPEN_SQUARE_BRACE = "[";
        protected const string CLOSE_SQUARE_BRACE = "]";
        protected const string QUOTE = "'";
        protected const string DOT = ".";

        public StatementParser( Tokenizer tokenizer )
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

        protected bool IsTokenInSet( string[] tokenSet )
        {
            foreach ( var token in tokenSet )
                if ( token.ToLower() == CurrentToken.ToLower() )
                    return true;

            return false;
        }

        protected Tokenizer Tokenizer { get; private set; }

        protected string GetIdentifier()
        {
            string identifier = "";
            if ( Tokenizer.TokenEquals( OPEN_SQUARE_BRACE ) )
            {
                identifier += OPEN_SQUARE_BRACE + CurrentToken + CLOSE_SQUARE_BRACE;
                ReadNextToken();
                ExpectToken( CLOSE_SQUARE_BRACE );
            }
            else
            {
                identifier = CurrentToken;
                ReadNextToken();
            }
            return identifier;
        }

        protected string GetTableName()
        {
            string tableName = "";
            do
            {
                string identifier = GetIdentifier();
                tableName += ( tableName != "" ? DOT : "" ) + identifier;
            }
            while ( Tokenizer.TokenEquals( DOT ) );

            return tableName;
        }

        protected string GetExpression()
        {
            if ( Tokenizer.TokenEquals( OPEN_BRACKET ) )
            {
                string result = GetExpression();
                ExpectToken( CLOSE_BRACKET );
                return result;
            }
            else
            {
                string token = "";
                if ( Tokenizer.TokenEquals( QUOTE ) )
                {
                    token += CurrentToken;
                    ReadNextToken();
                    ExpectToken( QUOTE );
                }
                else
                {
                    token = CurrentToken;
                    ReadNextToken();

                    // test whether the current token is a function - ie. SomeToken()
                    if ( Tokenizer.TokenEquals( OPEN_BRACKET ) )
                    {
                        // TODO: This needs to be implemented as a param list of expressions
                        token += "()";
                        ExpectToken( CLOSE_BRACKET );
                    }
                }
                return token;
            }
        }

        /// <summary>
        /// Returns an IStatement reference for the given statement type
        /// </summary>
        /// <returns></returns>
        public abstract IStatement Execute();
    }
}
