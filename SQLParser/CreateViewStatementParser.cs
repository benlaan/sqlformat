using System;

namespace Laan.SQL.Parser
{
    public class CreateViewStatementParser : StatementParser
    {
        private const string AS = "AS";
        private const string SELECT = "SELECT";
        CreateViewStatement _statement;

        internal CreateViewStatementParser( Tokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            ExpectTokens( AS, SELECT );

            _statement = new CreateViewStatement();

            SelectStatementParser parser = new SelectStatementParser( Tokenizer );
            _statement.SelectBlock = parser.Execute() as SelectStatement;

            return _statement;
        }
    }
}
