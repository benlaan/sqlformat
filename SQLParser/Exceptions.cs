using System;

namespace SQLParser
{
    public class ExpectedTokenNotFoundException : Exception
    {
        internal ExpectedTokenNotFoundException( string token, string foundToken )
            : base( "Expected: " + token + ", but found: " + foundToken ) { }
    }
}
