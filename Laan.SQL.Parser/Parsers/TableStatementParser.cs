using System;

namespace Laan.SQL.Parser
{
    public abstract class TableStatementParser<T> : StatementParser<T> where T : class, IStatement
    {
        internal TableStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }
    }
}
