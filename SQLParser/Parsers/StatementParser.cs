using System;

namespace Laan.SQL.Parser
{
    public class Constants
    {
        internal const string On = "ON";
        internal const string Comma = ",";
        internal const string OpenBracket = "(";
        internal const string CloseBracket = ")";
        internal const string OpenSquareBracket = "[";
        internal const string CloseSquareBracket = "]";
        internal const string Quote = "'";
        internal const string Dot = ".";
        internal const string Clustered = "CLUSTERED";
        internal const string NonClustered = "NONCLUSTERED";
        internal const string Unique = "UNIQUE";
        internal const string Index = "INDEX";
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
