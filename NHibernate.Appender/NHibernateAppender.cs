using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;
using System.IO;
using log4net.Layout;

namespace NHibernate.Appender
{
    public class SQLFormatter
    {
        string _sql;
        
        public SQLFormatter( string sql )
        {
            _sql = sql;
        }

        private void UpdateParamsWithValues( string paramList )
        {
            string[] paramItems = paramList.Split( ',' );
            foreach ( string item in paramItems )
            {
                string[] variables = item.Split( '=' );
                string name = variables[ 0 ].Trim();
                string value = variables[ 1 ].Trim();

                _sql = _sql.Replace( name, value );
            }
        }

        public string FormatSql()
        {
            string[] parts = _sql.Split( ';' );
            _sql = parts[ 0 ];

            if ( parts.Length > 1 )
            {
                string paramList = parts[ 1 ];
                UpdateParamsWithValues( paramList );
            }

            _sql = _sql.Replace( "select ", "SELECT\n\t" );
            _sql = _sql.Replace( "SELECT ", "SELECT\n\t" );
            _sql = _sql.Replace( ", ", ",\n\t" );
            _sql = _sql.Replace( " ,", "," );
            _sql = _sql.Replace( "from ", "\n\nFROM " );
            _sql = _sql.Replace( " FROM ", "\n\nFROM " );
            _sql = _sql.Replace( "left outer join ", "\n\nLEFT JOIN " );
            _sql = _sql.Replace( "inner join ", "\n\nJOIN " );
            _sql = _sql.Replace( "where ", "\n\nWHERE " );
            _sql = _sql.Replace( " WHERE ", "\n\nWHERE " );
            _sql = _sql.Replace( "AND", "\n  AND " );
            _sql = _sql.Replace( "OR", " OR " );
            _sql = _sql.Replace( " as ", " AS " );
            _sql = _sql.Replace( " on ", "\n  ON " );
            _sql = _sql.Replace( "group by  ", "\n\nGROUP BY\n\t" );
            _sql = _sql.Replace( "=", " = " );

            string line = new String( '-', 120 );
            return String.Format( "\n{0}\n-- {1}\n{0}\n{2}\n", line, DateTime.Now, _sql );
        }
    }

    public class NHibernateAppender : AppenderSkeleton
    {
        protected override void Append( LoggingEvent loggingEvent )
        {
            string formattedSql = FormatSql( loggingEvent.RenderedMessage );

            if ( !String.IsNullOrEmpty( FileName ) )
                File.AppendAllText( FileName, formattedSql );
        }

        protected override bool RequiresLayout
        {
            get { return false; }
        }

        private string FormatSql( string sql )
        {
            SQLFormatter formatter = new SQLFormatter( sql );
            return formatter.FormatSql();
        }

        public string FileName { get; set; }
    }
}
