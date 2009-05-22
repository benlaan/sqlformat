using System;

namespace Laan.SQL.Parser
{
    public class Constants
    {
        public const string On = "ON";
        public const string Comma = ",";
        public const string OpenBracket = "(";
        public const string CloseBracket = ")";
        public const string OpenSquareBracket = "[";
        public const string CloseSquareBracket = "]";
        public const string Quote = "'";
        public const string Dot = ".";
        public const string Clustered = "CLUSTERED";
        public const string NonClustered = "NONCLUSTERED";
        public const string Unique = "UNIQUE";
        public const string Index = "INDEX";
        public const string Select = "SELECT";
        public const string Insert = "INSERT";
        public const string Update = "UPDATE";
        public const string Delete = "DELETE";
        public const string All = "ALL";
        public const string To = "TO";
        public const string And = "AND";
        public const string Or = "OR";
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
