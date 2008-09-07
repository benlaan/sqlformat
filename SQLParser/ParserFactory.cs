using System;

namespace SQLParser
{
    public class ParserFactory
    {
        private const string SELECT = "SELECT";
        private const string INSERT = "INSERT";

        public static IStatement Execute( string sql )
        {
            Tokenizer _tokenizer = new Tokenizer( sql );
            IStatement _statement = null;

            while ( _tokenizer.HasMoreTokens )
            {
                _tokenizer.ReadNextToken();

                SelectStatementParser parser = null;

                if ( _tokenizer.TokenEquals( SELECT ) )
                    parser = new SelectStatementParser( _tokenizer );

                //if ( _tokenizer.TokenEquals( INSERT ) )
                //    parser = new InsertStatementParser( _tokenizer );

                //if ( _tokenizer.TokenEquals( UPDATE ) )
                //    parser = new UpdateStatementParser( _tokenizer );

                //if ( _tokenizer.TokenEquals( DELETE ) )
                //    parser = new DeleteStatementParser( _tokenizer );

                if ( parser == null )
                    throw new NotImplementedException( 
                        "No parser exists for that statement type: " + _tokenizer.Current 
                    );

                _statement = parser.Execute();
            }

            return _statement;
        }
    }
}
