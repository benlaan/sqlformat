using System;
using System.Collections.Generic;

namespace Laan.SQL.Parser
{
    public class CreateIndexParser : StatementParser<CreateIndexStatement>
    {
        public CreateIndexParser( ITokenizer tokenizer ) : base( tokenizer ) { }

        public override CreateIndexStatement Execute()
        {
            // CREATE [UNIQUE] [CLUSTERED | NONCLUSTERED] INDEX index_name ON table (column [,...n])

            _statement = new CreateIndexStatement();

            // optional
            if ( Tokenizer.TokenEquals( Constants.Unique ) )
                _statement.Unique = true;

            // optional
            if ( Tokenizer.TokenEquals( Constants.NonClustered ) )
                _statement.Clustered = false;

            // optional
            if ( Tokenizer.TokenEquals( Constants.Clustered ) )
                _statement.Clustered = true;
            
            Tokenizer.ExpectToken( Constants.Index );
            _statement.IndexName = GetDotNotationIdentifier();
            Tokenizer.ExpectToken( Constants.On );
            _statement.TableName = GetTableName();
            
            using ( Tokenizer.ExpectBrackets() )
            {
                _statement.Columns = GetIndexedColumnList();
            }

            return _statement;
        }

        private List<IndexedColumn> GetIndexedColumnList()
        {
            List<IndexedColumn> columns = new List<IndexedColumn>();
            do
            {
                string name = GetIdentifier();

                Order order = Order.Ascending;
                if ( Tokenizer.TokenEquals( Constants.Descending ) )
                    order = Order.Descending;
                else if ( Tokenizer.TokenEquals( Constants.Ascending ) )
                    order = Order.Ascending;

                columns.Add( new IndexedColumn() { Name = name, Order = order } );
            }
            while ( Tokenizer.TokenEquals( Constants.Comma ) );

            return columns;
        } 
    }
}
