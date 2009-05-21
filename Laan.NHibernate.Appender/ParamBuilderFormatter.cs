using System;

using Laan.SQL.Formatter;

namespace NHibernate.Appender
{
    public class ParamBuilderFormatter
    {
        private string UpdateParamsWithValues( string sql, string paramList )
        {
            string[] paramItems = paramList.Split( ',' );
            foreach ( string item in paramItems )
            {
                string[] variables = item.Split( '=' );
                string name = variables[ 0 ].Trim();
                string value = variables[ 1 ].Trim();

                sql = sql.Replace( name, value );
            }
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
            catch ( Exception )
            {
                return sql;
            }
        }

        public string GetFormattedSQL( string sql )
        {
            return FormatSql( sql );
        }
    }
}
