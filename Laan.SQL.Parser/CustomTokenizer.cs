using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Laan.SQL.Parser
{
    public class CustomTokenizer : ITokenizer
    {
        public bool IsNextToken( params string[] tokenSet )
        {
            foreach ( var token in tokenSet )
                if ( token.ToLower() == Current.ToLower() )
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
            bool areEqual = Current.ToLower() == value.ToLower();
            if ( areEqual )
                ReadNextToken();

            return areEqual;
        }

        public virtual void ReadNextToken()
        {
            // do nothing
        }

        public virtual string Current
        {
            get { return ""; }
        }

        public void ExpectToken( string token )
        {
            if ( Current.ToLower() != token.ToLower() )
                throw new ExpectedTokenNotFoundException( token, Current, Position );
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
