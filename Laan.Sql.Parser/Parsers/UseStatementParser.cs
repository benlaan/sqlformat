using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class UseStatementParser : StatementParser<UseStatement>
    {
        public UseStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public override UseStatement Execute()
        {
            return new UseStatement
            {
                DatabaseName = GetIdentifier()
            };
        }
    }
}
