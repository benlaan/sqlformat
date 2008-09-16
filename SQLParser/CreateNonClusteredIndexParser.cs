using System;

namespace Laan.SQL.Parser
{
    public class CreateNonClusteredIndexParser : StatementParser
    {
        private const string INDEX = "INDEX";
        private const string ON = "ON";

        public CreateNonClusteredIndexParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public override IStatement Execute()
        {
            CreateNonClusteredIndex statement = new CreateNonClusteredIndex();

            ExpectToken( INDEX );
            statement.ConstraintName = GetIdentifier();
            ExpectToken( ON );
            statement.TableName = GetTableName();
            ExpectToken( OPEN_BRACKET );
            statement.KeyName = GetIdentifier();
            ExpectToken( CLOSE_BRACKET );

            return statement;
        }
    }
}
