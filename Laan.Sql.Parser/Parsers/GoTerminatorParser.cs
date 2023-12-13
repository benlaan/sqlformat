using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class GoTerminatorParser : StatementParser<GoTerminator>
    {
        public GoTerminatorParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override GoTerminator Execute()
        {
            return new GoTerminator();
        }
    }
}
