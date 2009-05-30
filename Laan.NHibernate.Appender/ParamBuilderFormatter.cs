using System;
using System.Collections.Generic;

using Laan.SQL.Formatter;
using Laan.SQL.Parser;

namespace Laan.NHibernate.Appender
{
    public class ParamBuilderFormatter
    {
        private string UpdateParamsWithValues( string sql, string paramList )
        {
            var parameters = new Dictionary<string, string>();
            Tokenizer tokenizer = new Tokenizer( paramList );
            tokenizer.ReadNextToken();
            do
            {
                string parameter = tokenizer.Current;

                tokenizer.ReadNextToken();
                tokenizer.ExpectToken( "=" );

                parameters.Add( parameter, tokenizer.Current );
                tokenizer.ReadNextToken();
            } 
            while ( tokenizer.TokenEquals( Constants.Comma ) );

            foreach ( var item in parameters )
                sql = sql.Replace( item.Key, item.Value );

            return sql;
        }

        private string FormatSql( string sql )
        {
            // designed to convet the following format
            // "SELECT * FROM Table WHERE ID=@P1 AND Name=P2;@P1=20,@P2='Users'"
            string[] parts = sql.Split( ';' );
            if ( parts.Length > 1 )
                sql = UpdateParamsWithValues( parts[ 0 ], parts[ 1 ] );

            var engine = new FormattingEngine();
            try
            {
                return engine.Execute( sql );
            }
            catch ( Exception ex )
            {
                return String.Format( "Error: {0}\n{1}", ex.Message, sql );
            }
        }

        public string GetFormattedSQL( string sql )
        {
            return FormatSql( sql );
        }
    }
}
