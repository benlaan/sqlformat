using System;
using System.Collections.Generic;

using Laan.Sql.Parser.Entities;
using Laan.Sql.Parser.Exceptions;
using Laan.Sql.Parser.Expressions;

namespace Laan.Sql.Parser.Parsers
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

            // optional WITH
            if (Tokenizer.TokenEquals(Constants.With))
            {
                using (Tokenizer.ExpectBrackets())
                {
                    _statement.RelationalIndexOptions = GetRelationalIndexOptionsList();
                }
            }

            // optional ON
            if ( Tokenizer.TokenEquals( Constants.On ) )
            {
                _statement.FileGroupName = GetIdentifier();
            }
            return _statement;
        }

        private Dictionary<string, IndexWithOption> _optionMap = new Dictionary<string, IndexWithOption>()
        {
            {Constants.PadIndex, IndexWithOption.PadIndex},
            {Constants.SortInTempDb, IndexWithOption.SortInTempDb},
            {Constants.IgnoreDupKey, IndexWithOption.IgnoreDupKey},
            {Constants.StatisticsNorecompute, IndexWithOption.StatisticsNorecompute},
            {Constants.DropExisting, IndexWithOption.DropExisting},
            {Constants.Online, IndexWithOption.Online},
            {Constants.AllowRowLocks, IndexWithOption.AllowRowLocks},
            {Constants.AllowPageLocks, IndexWithOption.AllowPageLocks}
        };

        private List<RelationalIndexOption> GetRelationalIndexOptionsList()
        {
            var options = new List<RelationalIndexOption>();

            do
            {
                RelationalIndexOption option;

                if ( Tokenizer.IsNextToken( Constants.PadIndex, Constants.SortInTempDb, Constants.IgnoreDupKey, Constants.StatisticsNorecompute, Constants.DropExisting, Constants.Online, Constants.AllowRowLocks, Constants.AllowPageLocks ) )
                {
                    string optionName = Tokenizer.Current.Value;
                    Tokenizer.ReadNextToken();

                    Tokenizer.ExpectToken( Constants.Assignment );

                    if ( !Tokenizer.IsNextToken( Constants.On, Constants.Off ) )
                        throw new SyntaxException( "expects ON or OFF" );

                    option = new RelationalIndexOption();
                    option.Option = _optionMap[ optionName ];
                    option.Assignment = new StringExpression( Tokenizer.Current.Value, null );

                    Tokenizer.ReadNextToken();

                    options.Add( option );
                }

            } while ( Tokenizer.TokenEquals( Constants.Comma ) );

            return options;
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
