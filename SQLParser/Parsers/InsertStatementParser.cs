using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laan.SQL.Parser
{
    class InsertStatementParser : StatementParser
    {

    // INSERT [INTO] table_name [(column_list)] { 
    //      {
    //        VALUES ( { DEFAULT | NULL | expression }[,...n] )
    //        | derived_table
    //        | execute_statement    
    //      }

        public InsertStatementParser( Tokenizer tokenizer ) : base( tokenizer ) { }

        private InsertStatement _statement;

        public override IStatement Execute()
        {
            _statement = new InsertStatement();

            if ( Tokenizer.TokenEquals( "INTO" ) )
            {
                // consume the current token
            }

            _statement.TableName = GetTableName();

            if ( Tokenizer.TokenEquals( Constants.OpenBracket ) )
            {
                _statement.Columns = ( GetIdentifierList() );
                ExpectToken( Constants.CloseBracket );
            }

            if ( Tokenizer.TokenEquals( "VALUES" ) )
            {
                do
                {
                    ExpectToken( Constants.OpenBracket );
                    _statement.Values.Add( GetIdentifierList() );
                    ExpectToken( Constants.CloseBracket );
                }
                while ( Tokenizer.TokenEquals( Constants.Comma ) );
            }
            else
                if ( Tokenizer.IsNextToken( "SELECT" ) )
                {
                    ReadNextToken();
                    SelectExpression selectExpression = new SelectExpression();

                    var parser = new SelectStatementParser( Tokenizer );
                    _statement.SourceStatement = (SelectStatement) parser.Execute();
                }

            return _statement;
        }
    }
}
