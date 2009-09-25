using System;

namespace Laan.SQL.Parser
{
    public class ExpectedTokenNotFoundException : Exception
    {
        internal ExpectedTokenNotFoundException( string token, string foundToken, Position position )
            : base( "Expected: '" + token + "' but found: '" + foundToken + "' at " + position.ToString() ) { }
    }

    public class SyntaxException : Exception
    {
        public SyntaxException( string message ) : base( message ) { }
    }

    public class UnknownTokenException : Exception
    {
        public UnknownTokenException( string message ) : base( "'" + message + "'" )
        {

        }
    }

    public class ParserNotImplementedException : Exception
    {
        public ParserNotImplementedException( string message ) : base( message ){
        }
    }

    public class FormatterNotImplementedException : Exception
    {
        public FormatterNotImplementedException( string message ) : base( message )
        {
        }
    }
}
