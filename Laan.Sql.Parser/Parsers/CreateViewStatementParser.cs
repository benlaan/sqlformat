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
            _statement = new CreateViewStatement() { Name = GetDotNotationIdentifier(), IsAlter = IsAlter };

            ExpectToken(Constants.As);

            if (Tokenizer.IsNextToken(Constants.Select))
            {
                ReadNextToken();

                var parser = new SelectStatementParser(Tokenizer);
                _statement.Definition = parser.Execute();
            }
            else if (Tokenizer.IsNextToken(Constants.With))
            {
                ReadNextToken();

                var parser = new CommonTableExpressionStatementParser(Tokenizer);
                _statement.Definition = parser.Execute();
            }

            return _statement;
        }

        public bool IsAlter { get; set; }
    }
}
