using System;

namespace Laan.SQL.Parser
{
    public interface IParser
    {
        IStatement Execute();
    }

    /// <summary>
    /// Base class for parsing an SQL statement
    /// </summary>
    public abstract class StatementParser<T> : CustomParser, IParser where T : IStatement
    {
        protected T _statement;

        public StatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        protected string GetTableName()
        {
            return GetDotNotationIdentifier();
        }

        /// <summary>
        /// Returns an IStatement reference for the given statement type
        /// </summary>
        /// <returns></returns>
        public virtual T Execute()
        {
            return default( T );
        }


        #region IParser Members

        IStatement IParser.Execute()
        {
            return this.Execute() as IStatement;
        }

        #endregion
    }
}
