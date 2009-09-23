using System;
using System.Diagnostics;

namespace Laan.SQL.Parser
{
    public class CustomTokenizer : ITokenizer
    {
        public virtual bool IsNextToken( params string[] tokenSet )
        {
            foreach ( var token in tokenSet )
                if ( Current == token  )
                    return true;

            return false;
        }
        
        /// <summary>
        /// Utility (psuedo-function) to check that the current token equals the input parameter
        /// if so, the current token is advanced
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TokenEquals( string value )
        {
            bool areEqual = Current == value;
            if ( areEqual )
                ReadNextToken();

            return areEqual;
        }

        public virtual void ReadNextToken()
        {
            // do nothing
        }

        public virtual Token Current
        {
            get { return new Token(); }
        }

        public void ExpectToken( string token )
        {
            if ( Current != token )
                throw new ExpectedTokenNotFoundException( token, Current.Value, Position );
            else
                ReadNextToken();
        }

        public void ExpectTokens( string[] tokens )
        {
            foreach ( string token in tokens )
                ExpectToken( token );
        }

        public Position Position { get; protected set; }

        public BracketStructure ExpectBrackets()
        {
            return new BracketStructure( this );
        }

        public virtual bool HasMoreTokens
        {
            get { return false; }
        }
    }
}
