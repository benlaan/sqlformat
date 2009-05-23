using System;
using System.Collections.Generic;

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
            
            using ( Tokenizer.ExpectBrackets() )
            {
                statement.Columns = GetIndexedColumnList();
            }

            return statement;
        }

        private List<IndexedColumn> GetIndexedColumnList()
        {
            List<IndexedColumn> columns = new List<IndexedColumn>();
            do
            {
                string name = GetIdentifier();

                Order order = Order.Ascending;
                if ( Tokenizer.TokenEquals( "DESC" ) )
                    order = Order.Descending;

                if ( Tokenizer.TokenEquals( "ASC" ) )
                    order = Order.Ascending;

                columns.Add( new IndexedColumn() { Name = name, Order = order } );
            }
            while ( Tokenizer.TokenEquals( Constants.Comma ) );

            return columns;
        } 
    }
}
