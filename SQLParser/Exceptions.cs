using System;

namespace Laan.SQL.Parser
{
    public class ExpectedTokenNotFoundException : Exception
    {
        internal ExpectedTokenNotFoundException( string token, string foundToken )
            : base( "Expected: [" + token + "], but found: [" + foundToken + "]" ) { }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException( string message ) : base( message ) { }
    }
}
