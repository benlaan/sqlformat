using System;

namespace Laan.SQL.Parser
{
    public class CreateIndexParser : StatementParser
    {
        public CreateIndexParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        public override IStatement Execute()
        {
            // CREATE [UNIQUE] [CLUSTERED | NONCLUSTERED] INDEX index_name ON table (column [,...n])

            CreateIndex statement = new CreateIndex();

            // optional
            if ( Tokenizer.TokenEquals( Constants.Unique ) )
                statement.Unique = true;

            // optional
            if ( Tokenizer.TokenEquals( Constants.NonClustered ) )
                statement.Clustered = false;

            // optional
            if ( Tokenizer.TokenEquals( Constants.Clustered ) )
                statement.Clustered = true;
            
            Tokenizer.ExpectToken( Constants.Index );
            statement.IndexName = GetIdentifier();
            Tokenizer.ExpectToken( Constants.On );
            statement.TableName = GetTableName();
            
            Tokenizer.ExpectToken( Constants.OpenBracket );
            statement.Columns = GetIdentifierList();
            Tokenizer.ExpectToken( Constants.CloseBracket );

            return statement;
        }
    }
}
