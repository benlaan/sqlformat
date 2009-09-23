using System;

namespace Laan.SQL.Parser
{
    public class CreateViewStatementParser : StatementParser
    {
        private const string AS = "AS";
        private const string SELECT = "SELECT";
        CreateViewStatement _statement;

        internal CreateViewStatementParser( ITokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            _statement = new CreateViewStatement();
            _statement.Name = GetIdentifier();

            ExpectTokens( AS, SELECT );

            SelectStatementParser parser = new SelectStatementParser( Tokenizer );
            _statement.SelectBlock = parser.Execute() as SelectStatement;

            return _statement;
        }
    }
}
