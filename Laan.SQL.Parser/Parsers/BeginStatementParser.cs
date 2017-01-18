using System;

using Laan.Sql.Parser.Entities;

namespace Laan.Sql.Parser.Parsers
{
    public class BeginStatementParser : CustomParser, IParser
    {
        public BeginStatementParser(ITokenizer tokenizer) : base(tokenizer)
        {
        }

        public IStatement Execute()
        {
            if (Tokenizer.IsNextToken(Constants.Tran, Constants.Transaction, Constants.Distributed))
                return new BeginTransactionStatementParser(Tokenizer).Execute();

            var statement = new BlockStatement();
            statement.Statements.AddRange(ParserFactory.Execute(Tokenizer, false));
            Tokenizer.ExpectToken(Constants.End);
            return statement;
        }
    }
}
