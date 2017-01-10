using System;
using System.Collections.Generic;
using System.Linq;

using Laan.Sql.Parser;
using Laan.Sql.Parser.Parsers;
using Laan.Sql.Parser.Exceptions;

namespace Laan.Sql.Parser
{
    public class ParserFactory
    {
        private static Dictionary<string, Type> _parsers;

        /// <summary>
        /// Initializes a new instance of the ParserFactory class.
        /// </summary>
        static ParserFactory()
        {
            _parsers = new Dictionary<string, Type>
            {
                { Constants.Select,   typeof( SelectStatementParser   ) },
                { Constants.Insert,   typeof( InsertStatementParser   ) },
                { Constants.Update,   typeof( UpdateStatementParser   ) },
                { Constants.Delete,   typeof( DeleteStatementParser   ) },
                { Constants.Grant,    typeof( GrantStatementParser    ) },
                { Constants.Go,       typeof( GoTerminatorParser      ) },
                { Constants.Create,   typeof( CreateStatementParser   ) },
                { Constants.Alter,    typeof( AlterStatementParser    ) },
                { Constants.Declare,  typeof( DeclareStatementParser  ) },
                { Constants.If,       typeof( IfStatementParser       ) },
                { Constants.Begin,    typeof( BeginStatementParser    ) },
                { Constants.Commit,   typeof( CommitStatementParser   ) },
                { Constants.Rollback, typeof( RollbackStatementParser ) },
                { Constants.Set,      typeof( SetStatementParser      ) },
                { Constants.Exec,     typeof( ExecStatementParser     ) },
                { Constants.With,     typeof( CommonTableExpressionStatementParser ) }
            };
        }

        internal static IParser GetParser( ITokenizer _tokenizer )
        {
            // this is a quick and dirty service locator that maps tokens to parsers
            Type parserType;
            if ( _parsers.TryGetValue( _tokenizer.Current.Value.ToUpper(), out parserType ) )
            {
                _tokenizer.ReadNextToken();

                object instance = Activator.CreateInstance( parserType, _tokenizer );
                return (IParser) instance;
            }
            return null;
        }
        
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

        public static List<IStatement> Execute( ITokenizer tokenizer, bool ensureParserIsFound )
        {
            var result = new List<IStatement>();

            if ( tokenizer.Current == (Token) null )
                tokenizer.ReadNextToken();

            while ( tokenizer.HasMoreTokens )
            {
                IParser parser = GetParser( tokenizer );

                if ( parser != null )
                    result.Add( parser.Execute() );
                else
                    if ( ensureParserIsFound )
                        throw new ParserNotImplementedException(
                            "No parser exists for statement type: " + tokenizer.Current.Value
                        );
                    else
                        break;
            }

            return result;
        }

        /// <summary>
        /// This will parse any statement, and return only the interface (IStatement)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static List<IStatement> Execute( string sql )
        {
            sql = TrimBatchMetadata(sql);

            using ( var sqlTokenizer = new SqlTokenizer( sql ) )
            {
                return Execute( sqlTokenizer, true );
            }
        }

        // clean NHibernate info from front of SQL statement
        public static string TrimBatchMetadata(string sql)
        {
            const string batchCommands = "Batch commands:\r\n";
            if (sql.StartsWith(batchCommands))
            {
                sql = sql.Remove(0, batchCommands.Length);

                const string command = "command";

                if (sql.StartsWith(command))
                {
                    int colonPos = sql.IndexOf(":", StringComparison.Ordinal);

                    if (colonPos > 0)
                        sql = sql.Remove(0, colonPos + 1);
                }
            }
            return sql;
        }
    }
}
