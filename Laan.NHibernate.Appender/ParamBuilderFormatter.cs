using System;
using System.Collections.Generic;

using Laan.SQL.Formatter;
using Laan.SQL.Parser;

namespace Laan.NHibernate.Appender
{
    public class ParamBuilderFormatter
    {
        private IFormattingEngine _engine;

        /// <summary>
        /// Initializes a new instance of the ParamBuilderFormatter class.
        /// </summary>
        /// <param name="engine"></param>
        public ParamBuilderFormatter( IFormattingEngine engine )
        {
            _engine = engine;
        }

        private static string ProcessParameter( ITokenizer tokenizer )
        {
            // the following code produces a parameter that supports Guids. For unknown reasons, NH
            // is supplying the parameter value as an unquoted set of alpha numerics, so, here they are processed 
            // until the next token is NOT a dash
            int tokenCount = 0;
            string token = "";
            do
            {
                token += tokenizer.Current.Value;
                tokenizer.ReadNextToken();
                tokenCount++;
            }
            while ( tokenizer.HasMoreTokens && !tokenizer.IsNextToken( Constants.Comma ) );

            return tokenCount > 1 ? String.Format( "'{0}'", token.ToUpper() ) : token;
        }

        private string UpdateParamsWithValues( string sql, string paramList )
        {
            var parameters = new Dictionary<string, string>();
            ITokenizer tokenizer = new SqlTokenizer( paramList );
            tokenizer.ReadNextToken();
            do
            {
                string parameter = tokenizer.Current.Value;

                tokenizer.ReadNextToken();
                tokenizer.ExpectToken( Constants.Assignment );

                parameters.Add( parameter, ProcessParameter( tokenizer ) );
            }
            while ( tokenizer.TokenEquals( Constants.Comma ) );

            foreach ( var item in parameters )
                sql = sql.Replace( item.Key, item.Value );

            return sql;
        }

        public string Execute( string sql )
        {
            // designed to convert the following format
            // "SELECT * FROM Table WHERE ID=@P1 AND Name=@P2;@P1=20,@P2='Users'"
            string[] parts = sql.Split( ';' );
            if ( parts.Length > 1 )
                sql = UpdateParamsWithValues( parts[ 0 ], parts[ 1 ] );

            try
            {
                return _engine.Execute( sql );
            }
            catch ( Exception ex )
            {
                return String.Format( "-- Error: {0}\n{1}", ex.Message, sql );
            }
        }
    }
}
