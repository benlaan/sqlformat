using System;

namespace Laan.Sql.Parser.Exceptions
{
    public abstract class ParserException : Exception
    {
        public ParserException() : base()
        {
        }

        public ParserException(string message) : base(message)
        {
        }

        public ParserException(string message, Exception inner) : base(message, inner)
        {
        }
    }

    public class ExpectedTokenNotFoundException : ParserException
    {
        internal ExpectedTokenNotFoundException(string token, string foundToken, Position position)
            : base("Expected: '" + token + "' but found: '" + foundToken + "' at " + position.ToString()) { }

        public ExpectedTokenNotFoundException() : base() { }

        public ExpectedTokenNotFoundException(string message) : base(message) { }

        public ExpectedTokenNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class SyntaxException : ParserException
    {
        public SyntaxException() : base() { }
        public SyntaxException(string message) : base(message) { }
        public SyntaxException(string message, Exception innerException) : base(message, innerException) { }

    }

    public class UnknownTokenException : ParserException
    {
        public UnknownTokenException() : base() { }
        public UnknownTokenException(string message) : base("'" + message + "'") { }
        public UnknownTokenException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ParserNotImplementedException : ParserException
    {
        public ParserNotImplementedException() : base() { }
        public ParserNotImplementedException(string message) : base(message) { }
        public ParserNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class FormatterNotImplementedException : ParserException
    {
        public FormatterNotImplementedException() : base() { }
        public FormatterNotImplementedException(string message) : base(message) { }
        public FormatterNotImplementedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
