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

            Tokenizer.ExpectToken( INDEX );
            statement.ConstraintName = GetIdentifier();
            Tokenizer.ExpectToken( ON );
            statement.TableName = GetTableName();
            Tokenizer.ExpectToken( Constants.OPEN_BRACKET );
            statement.KeyName = GetIdentifier();
            Tokenizer.ExpectToken( Constants.CLOSE_BRACKET );

            return statement;
        }
    }
}
