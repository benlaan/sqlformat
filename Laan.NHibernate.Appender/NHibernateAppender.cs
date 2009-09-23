using System;
using System.IO;

using log4net.Appender;
using log4net.Core;

using Laan.SQL.Formatter;

namespace Laan.NHibernate.Appender
{
    public class NHibernateAppender : RollingFileAppender
    {
        private ParamBuilderFormatter _formatter;

        public NHibernateAppender()
        {
            _formatter = new ParamBuilderFormatter( new FormattingEngine() );
        }

        protected override void Append( LoggingEvent loggingEvent )
        {
            var data = new LoggingEventData 
            { 
                Message = _formatter.Execute( loggingEvent.RenderedMessage ),
                TimeStamp = loggingEvent.TimeStamp,
                Level = loggingEvent.Level,
                LoggerName = loggingEvent.LoggerName,
                ThreadName = loggingEvent.ThreadName,
                UserName = loggingEvent.UserName,
                Identity = loggingEvent.Identity,
                Domain = loggingEvent.Domain
            };
            base.Append( new LoggingEvent( data ) );
        }
    }
}
