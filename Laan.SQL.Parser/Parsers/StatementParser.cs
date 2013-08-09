using System;
using System.Linq;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
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

        private readonly string[] _tableHints = 
        {
            Constants.NoExpand,
            Constants.FastFirstrow,
            Constants.ForceSeek,
            Constants.HoldLock,
            Constants.NoLock,
            Constants.NoWait,
            Constants.PagLock,
            Constants.ReadCommitted,
            Constants.ReadCommittedLock,
            Constants.ReadPast,
            Constants.ReadUncommitted,
            Constants.RepeatableRead,
            Constants.RowLock,
            Constants.Serializable,
            Constants.TabLock,
            Constants.TabLockx,
            Constants.UpdLock,
            Constants.XLock,
        };

        private readonly string[] _tableHintsSingle = 
        {
            Constants.NoLock,
            Constants.ReadUncommitted,
            Constants.UpdLock,
            Constants.RepeatableRead,
            Constants.Serializable,
            Constants.ReadCommitted,
            Constants.FastFirstrow,
            Constants.TabLock,
            Constants.TabLockx,
            Constants.PagLock,
            Constants.RowLock,
            Constants.NoWait,
            Constants.ReadPast,
            Constants.XLock,
            Constants.NoExpand
        };

        protected StatementParser(ITokenizer tokenizer) : base(tokenizer) { }

        protected string GetTableName()
        {
            return GetDotNotationIdentifier();
        }

        protected void ProcessTerminator()
        {
            _statement.Terminated = HasTerminator();
        }

        /// <summary>
        /// Returns an IStatement reference for the given statement type
        /// </summary>
        /// <returns></returns>
        public virtual T Execute()
        {
            return default(T);
        }

        #region IParser Members

        IStatement IParser.Execute()
        {
            return this.Execute() as IStatement;
        }

        #endregion
        public enum With
        {
            Optional,
            Required
        }

        protected void ProcessTableHints(ITableHints hintable, With with = With.Optional)
        {
            if (with == With.Optional && Tokenizer.Current == Constants.OpenBracket)
            {
                using (Tokenizer.ExpectBrackets())
                {
                    ProcessHint(hintable);
                }
            }
            else if (Tokenizer.TokenEquals(Constants.With))
            {
                if (Tokenizer.IsNextToken(Constants.OpenBracket))
                {
                    using (Tokenizer.ExpectBrackets())
                    {
                        do
                        {
                            ProcessHint(hintable);
                        }
                        while (Tokenizer.TokenEquals(Constants.Comma));
                    }
                }
            }
        }

        private void ProcessHint(ITableHints hintable)
        {
            if (Tokenizer.IsNextToken(_tableHints))
            {
                var hint = new TableHint { Hint = Tokenizer.Current.Value };
                hintable.TableHints.Add(hint);
                Tokenizer.ReadNextToken();
            }
        }
    }
}
