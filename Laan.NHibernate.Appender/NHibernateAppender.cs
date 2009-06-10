using System;
using System.IO;

using log4net.Appender;
using log4net.Core;

using Laan.SQL.Formatter;

namespace Laan.NHibernate.Appender
{
    public class NHibernateAppender : AppenderSkeleton
    {
        private ParamBuilderFormatter _impl;

        public NHibernateAppender()
        {
            _impl = new ParamBuilderFormatter( new FormattingEngine() );
        }

        protected override void Append( LoggingEvent loggingEvent )
        {
            string formattedSql = String.Format( 
                "{0}\n{1}\n{0}\n{2}\n", 
                new string( '-', 80 ),
                loggingEvent.TimeStamp,
                _impl.Execute( loggingEvent.RenderedMessage ) 
            );

            if ( !String.IsNullOrEmpty( FileName ) )
                File.AppendAllText( FileName, formattedSql );
        }

        protected override bool RequiresLayout
        {
            get { return false; }
        }

        public string FileName { get; set; }
    }
}
