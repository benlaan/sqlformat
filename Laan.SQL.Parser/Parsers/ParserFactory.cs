using System;
using System.Collections.Generic;
using System.Linq;

namespace Laan.SQL.Parser
{
    public class ParserFactory
    {
        private const string SELECT = "SELECT";
        private const string INSERT = "INSERT";
        private const string UPDATE = "UPDATE";
        private const string TABLE = "TABLE";
        private const string CREATE = "CREATE";
        private const string ALTER = "ALTER";
        private const string VIEW = "VIEW";
        private const string GRANT = "GRANT";

        /// <summary>
        /// This method is used if you know what type will be returned from the parser
        /// - only use it if 100% confident, otherwise you will get a null reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<T> Execute<T>( string sql ) where T : class, IStatement
        {
            List<IStatement> list = Execute( sql );
            return list.Cast<T>().ToList<T>();
        }

        private static ITokenizer GetTokenizer( string sql )
        {
            return new SqlTokenizer( sql );
        }

        /// <summary>
        /// This will parse any statement, and return only the interface (IStatement)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<IStatement> Execute( string sql )
        {
            var result = new List<IStatement>();
            ITokenizer _tokenizer = GetTokenizer( sql );
    
            if ( _tokenizer.Current == (Token)null )
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

                    if ( _tokenizer.IsNextToken( Constants.Unique, Constants.Clustered, Constants.NonClustered, Constants.Index ) )
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
                else 
                if ( _tokenizer.TokenEquals( INSERT ) )
                    parser = new InsertStatementParser( _tokenizer );
                else 
                if ( _tokenizer.TokenEquals( UPDATE ) )
                    parser = new UpdateStatementParser( _tokenizer );

                //else if ( _tokenizer.TokenEquals( DELETE ) )
                //    parser = new DeleteStatementParser( _tokenizer );

                if ( parser == null && _tokenizer.Current != (Token) null )
                    throw new NotImplementedException(
                        "No parser exists for statement type: " + _tokenizer.Current.Value
                    );

                result.Add( parser.Execute() );
            }

            return result;
        }
    }
}
