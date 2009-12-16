using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

using log4net.Appender;
using log4net.Core;

using Laan.SQL.Formatter;

namespace Laan.NHibernate.Appender
{
    public class NHibernateAppender : RollingFileAppender
    {
        private object _latch;
        private bool _done = false;
        private BackgroundWorker _worker;
        private Queue<LoggingEvent> _queue;
        private ParamBuilderFormatter _formatter;

        public NHibernateAppender()
        {
            _latch = new object();
            _formatter = new ParamBuilderFormatter( new FormattingEngine() );
            _queue = new Queue<LoggingEvent>();

            _worker = new BackgroundWorker();
            _worker.DoWork += ProcessQueue;
            _worker.RunWorkerAsync();
        }

        private void ProcessQueue( object sender, DoWorkEventArgs e )
        {
            while ( !_done )
            {
                LoggingEvent loggingEvent = null;
                lock ( _latch )
                    if ( _queue.Count > 0 )
                        loggingEvent = _queue.Dequeue();

                if ( loggingEvent != null )
                {
                    var timer = Stopwatch.StartNew();
                    string formattedStatement = _formatter.Execute( loggingEvent.RenderedMessage );
                    timer.Stop();
                    
                    string message = String.Format(
                        "{0}\n-- Duration: {1:0:00:0000}", formattedStatement, timer.ElapsedMilliseconds
                    );

                    var data = new LoggingEventData
                    {
                        Message     = message,
                        TimeStamp   = loggingEvent.TimeStamp,
                        Level       = loggingEvent.Level,
                        LoggerName  = loggingEvent.LoggerName,
                        ThreadName  = loggingEvent.ThreadName,
                        UserName    = loggingEvent.UserName,
                        Identity    = loggingEvent.Identity,
                        Domain      = loggingEvent.Domain
                    };
                    base.Append( new LoggingEvent( data ) );
                }
                Thread.Sleep( 200 );
            }
        }

        protected override void Append( LoggingEvent loggingEvent )
        {
            lock ( _latch )
                _queue.Enqueue( loggingEvent );
        }

        protected override void OnClose()
        {
            _done = true;
            base.OnClose();
        }
    }
}
