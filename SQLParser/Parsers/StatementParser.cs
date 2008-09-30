using System;

namespace Laan.SQL.Parser
{
    public class Constants
    {
        internal const string COMMA = ",";
        internal const string OPEN_BRACKET = "(";
        internal const string CLOSE_BRACKET = ")";
        internal const string OPEN_SQUARE_BRACE = "[";
        internal const string CLOSE_SQUARE_BRACE = "]";
        internal const string QUOTE = "'";
        internal const string DOT = ".";
    }

    /// <summary>
    /// Base class for parsing an SQL statement
    /// </summary>
    public abstract class StatementParser : CustomParser
    {

        public StatementParser( Tokenizer tokenizer ) : base ( tokenizer ) { }

        protected string GetTableName()
        {
            return GetDotNotationIdentifier();
        }

        /// <summary>
        /// Returns an IStatement reference for the given statement type
        /// </summary>
        /// <returns></returns>
        public abstract IStatement Execute();
    }
}
