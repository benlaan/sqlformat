using System;

namespace Laan.SQL.Parser
{
    public class ParserFactory
    {
        private const string SELECT = "SELECT";
        private const string INSERT = "INSERT";
        private const string TABLE = "TABLE";
        private const string CREATE = "CREATE";
        private const string ALTER = "ALTER";
        private const string VIEW = "VIEW";
        private const string GRANT = "GRANT";

        /// <summary>
        /// This method is used if you know what type will be returned from the parser
        /// - only use it if 100% confident, otherwise you will get an exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static T Execute<T>( string sql ) where T : class, IStatement
        {
            return Execute( sql ) as T;
        }

        /// <summary>
        /// This will parse any statement, and return only the interface (IStatement)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static IStatement Execute( string sql )
        {
            Tokenizer _tokenizer = new Tokenizer( sql );
            IStatement _statement = null;

            if ( ( String.IsNullOrEmpty( _tokenizer.Current ) ) )
                _tokenizer.ReadNextToken();

            while ( _tokenizer.HasMoreTokens )
            {
                StatementParser parser = null;

                if ( _tokenizer.TokenEquals( SELECT ) )
                    parser = new SelectStatementParser( _tokenizer );

                else if ( _tokenizer.TokenEquals( CREATE ) )
                {
                    if ( _tokenizer.TokenEquals( TABLE ) )
                        parser = new CreateTableStatementParser( _tokenizer );

                    if ( _tokenizer.TokenEquals( VIEW ) )
                        parser = new CreateViewStatementParser( _tokenizer );

                    if ( _tokenizer.IsNextToken( Constants.Unique, Constants.Clustered, Constants.NonClustered ) )
                        parser = new CreateIndexParser( _tokenizer );
                }
                else if ( _tokenizer.TokenEquals( GRANT ) )
                {
                    parser = new GrantParser( _tokenizer );
                }
                else if ( _tokenizer.TokenEquals( ALTER ) )
                {
                    if ( _tokenizer.TokenEquals( TABLE ) )
                        parser = new AlterTableStatementParser( _tokenizer );
                }
                else if ( _tokenizer.TokenEquals( INSERT ) )
                    parser = new InsertStatementParser( _tokenizer );

                //if ( _tokenizer.TokenEquals( UPDATE ) )
                //    parser = new UpdateStatementParser( _tokenizer );

                //if ( _tokenizer.TokenEquals( DELETE ) )
                //    parser = new DeleteStatementParser( _tokenizer );

                if ( parser == null && _tokenizer.Current != null )
                    throw new NotImplementedException(
                        "No parser exists for statement type: " + _tokenizer.Current
                    );

                if ( _tokenizer.Current != null )
                    _statement = parser.Execute();

                _tokenizer.ReadNextToken();
            }

            return _statement;
        }
    }
}
