using System;

namespace Laan.SQL.Parser
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
