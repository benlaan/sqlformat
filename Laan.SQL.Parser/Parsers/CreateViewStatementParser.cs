using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class CreateViewStatementParser : StatementParser<CreateViewStatement>
    {
        internal CreateViewStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public override CreateViewStatement Execute()
        {
            _statement = new CreateViewStatement() { Name = GetDotNotationIdentifier() };

            ExpectToken(Constants.As);

            if (Tokenizer.IsNextToken(Constants.Select))
            {
                ReadNextToken();

                var parser = new SelectStatementParser(Tokenizer);
                _statement.ScriptBlock = parser.Execute();
            }
            else if (Tokenizer.IsNextToken(Constants.With))
            {
                ReadNextToken();

                var parser = new CommonTableExpressionStatementParser(Tokenizer);
                _statement.ScriptBlock = parser.Execute();
            }

            return _statement;
        }
    }
}
