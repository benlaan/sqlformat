using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser.Parsers
{
    public class DeclareStatementParser : StatementParser<DeclareStatement>
    {
        public DeclareStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
            _statement = new DeclareStatement();
        }

        public override DeclareStatement Execute()
        {
            if (Tokenizer.HasMoreTokens)
            {
                var parser = new VariableDefinitionParser(Tokenizer);
                _statement.Definitions = parser.Execute();
            }

            if (_statement.Definitions.Count == 0)
                throw new SyntaxException("DECLARE requires at least one variable declaration");

            return _statement;
        }
    }
}