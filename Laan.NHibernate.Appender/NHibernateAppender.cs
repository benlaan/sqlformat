using System;
using System.IO;

using log4net.Appender;
using log4net.Core;

using Laan.SQL.Formatter;

namespace NHibernate.Appender
{
    public class NHibernateAppender : AppenderSkeleton
    {
        private ParamBuilderFormatter _impl;

        public NHibernateAppender()
        {
            _impl = new ParamBuilderFormatter();
        }

        protected override void Append( LoggingEvent loggingEvent )
        {
            string formattedSql = _impl.GetFormattedSQL( loggingEvent.RenderedMessage );

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
