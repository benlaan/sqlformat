using System;

namespace Laan.SQL.Parser
{
    public abstract class TableStatementParser : StatementParser
    {
        protected const string CONSTRAINT = "CONSTRAINT";
        protected const string PRIMARY = "PRIMARY";
        protected const string KEY = "KEY";
        protected const string CLUSTERED = "CLUSTERED";

        internal TableStatementParser( CustomTokenizer tokenizer ) : base( tokenizer ) { }
    }
}
