using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Parsers;
using Laan.Sql.Parser;

namespace Laan.SQL.Parser
{
    public class SetStatementParser : StatementParser<SetStatement>
    {
        public SetStatementParser( ITokenizer tokenizer) : base ( tokenizer )
        {
            
        }

        public override SetStatement Execute()
        {
            SetStatement statement = new SetStatement();
            statement.Variable = GetIdentifier();
            Tokenizer.ExpectToken( Constants.Assignment );
            statement.Expression = new ExpressionParser( Tokenizer ).Execute();
            return statement;
        }

    }
}
