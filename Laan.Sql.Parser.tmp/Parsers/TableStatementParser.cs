using System;

namespace Laan.Sql.Parser.Parsers
{
    public abstract class TableStatementParser<T> : StatementParser<T> where T : class, IStatement
    {
        internal TableStatementParser( ITokenizer tokenizer ) : base( tokenizer ) { }
    }
}
