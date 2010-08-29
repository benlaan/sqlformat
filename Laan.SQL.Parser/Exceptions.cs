using System;

namespace Laan.Sql.Parser.Exceptions
{
    [Serializable]
    public class ExpectedTokenNotFoundException : Exception
    {
        internal ExpectedTokenNotFoundException( string token, string foundToken, Position position )
            : base( "Expected: '" + token + "' but found: '" + foundToken + "' at " + position.ToString() ) { }
    }

    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException( string message ) : base( message ) { }
    }


    [Serializable]
    public class UnknownTokenException : Exception
    {
        public UnknownTokenException( string message ) : base( "'" + message + "'" )
        {

        }
    }

    [Serializable]
    public class ParserNotImplementedException : Exception
    {
        public ParserNotImplementedException( string message ) : base( message ){
        }
    }

    [Serializable]
    public class FormatterNotImplementedException : Exception
    {
        public FormatterNotImplementedException( string message ) : base( message )
        {
        }
    }
}
