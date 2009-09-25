using System;
using System.Collections.Generic;
using System.Linq;

namespace Laan.SQL.Parser
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
                { Constants.Select, typeof( SelectStatementParser ) },
                { Constants.Insert, typeof( InsertStatementParser ) },
                { Constants.Update, typeof( UpdateStatementParser ) },
                { Constants.Delete, typeof( DeleteStatementParser ) },
                { Constants.Grant,  typeof( GrantStatementParser  ) },
                { Constants.Go,     typeof( GoTerminatorParser    ) },
                { Constants.Create, typeof( CreateStatementParser ) },
                { Constants.Alter,  typeof( AlterStatementParser  ) },
            };
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

            if ( _tokenizer.Current == (Token) null )
                _tokenizer.ReadNextToken();

            while ( _tokenizer.HasMoreTokens )
            {
                IParser parser = GetParser( _tokenizer );

                if ( parser == null )
                    throw new ParserNotImplementedException(
                        "No parser exists for statement type: " + _tokenizer.Current.Value
                    );

                result.Add( parser.Execute() );
            }

            return result;
        }

        private static IParser GetParser( ITokenizer _tokenizer )
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
    }
}
