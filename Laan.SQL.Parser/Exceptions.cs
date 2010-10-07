using System;

namespace Laan.Sql.Parser.Exceptions
{
    [Serializable]
    public class ExpectedTokenNotFoundException : Exception
    {
        internal ExpectedTokenNotFoundException( string token, string foundToken, Position position )
            : base( "Expected: '" + token + "' but found: '" + foundToken + "' at " + position.ToString() ) { }

        public ExpectedTokenNotFoundException() : base() { }

        public ExpectedTokenNotFoundException( string message ) : base( message ) { }

        public ExpectedTokenNotFoundException( string message, Exception innerException ) : base( message, innerException ) { }
    }

    [Serializable]
    public class SyntaxException : Exception
    {
        public SyntaxException() : base() { }
        public SyntaxException( string message ) : base( message ) { }
        public SyntaxException( string message, Exception innerException ) : base( message, innerException ) { }

    }

    [Serializable]
    public class UnknownTokenException : Exception
    {
        public UnknownTokenException() : base() { }
        public UnknownTokenException( string message ) : base( "'" + message + "'" ) { }
        public UnknownTokenException( string message, Exception innerException ) : base( message, innerException ) { }
    }

    [Serializable]
    public class ParserNotImplementedException : Exception
    {
        public ParserNotImplementedException() : base() { }
        public ParserNotImplementedException( string message ) : base( message ) { }
        public ParserNotImplementedException( string message, Exception innerException ) : base( message, innerException ) { }
    }

    [Serializable]
    public class FormatterNotImplementedException : Exception
    {
        public FormatterNotImplementedException() : base() { }
        public FormatterNotImplementedException( string message ) : base( message ) { }
        public FormatterNotImplementedException( string message, Exception innerException ) : base( message, innerException ) { }
    }
}
