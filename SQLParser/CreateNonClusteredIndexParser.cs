using System;

namespace Laan.SQL.Parser
{
    public class CreateNonClusteredIndexParser : StatementParser
    {
        private const string INDEX = "INDEX";
        private const string ON = "ON";

        public CreateNonClusteredIndexParser( Tokenizer tokenizer ) : base( tokenizer )
        {
        }

        public override IStatement Execute()
        {
            ExpectToken( INDEX );
            string name = GetIdentifier();
            ExpectToken( ON );
            string tableName = GetTableName();
            ExpectToken( OPEN_BRACKET );
            string keyName = GetIdentifier();
            ExpectToken( CLOSE_BRACKET );

            // TODO: There is current no actual CreateNonClusteredIndex statement to return
            return null;
        }
    }
}
