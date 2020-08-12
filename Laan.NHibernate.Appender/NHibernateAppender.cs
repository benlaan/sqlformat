using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Laan.Sql.Formatter;

using log4net.Appender;
using log4net.Core;

namespace Laan.NHibernate.Appender
{
    public class NHibernateAppender : RollingFileAppender, IDisposable
    {
        private object _latch;
        private bool _done = false;
        private Task _worker;
        private Queue<LoggingEvent> _queue;
        private ParamBuilderFormatter _formatter;
        private CancellationTokenSource _cancellationToken;

        public NHibernateAppender()
        {
            _latch = new object();
            _formatter = new ParamBuilderFormatter(new FormattingEngine());
            _queue = new Queue<LoggingEvent>();
            _cancellationToken = new CancellationTokenSource();
            _worker = Task.Factory.StartNew(() => ProcessQueue(200), _cancellationToken.Token);
        }

        public override void ActivateOptions()
        {
            File = Environment.ExpandEnvironmentVariables(File);
            base.ActivateOptions();
        }

        private void ProcessQueue(int delay)
        {
            while (!_done)
            {
                LoggingEvent loggingEvent = null;
                lock (_latch)
                    if (_queue.Count > 0)
                        loggingEvent = _queue.Dequeue();

                if (loggingEvent != null)
                {
                    var timer = Stopwatch.StartNew();
                    string formattedStatement = _formatter.Execute(loggingEvent.RenderedMessage);
                    timer.Stop();

                    string message = $"{formattedStatement}\n-- Duration: {timer.ElapsedMilliseconds:0:00:0000}";

                    var data = new LoggingEventData
                    {
                        Message = message,
                        TimeStampUtc = loggingEvent.TimeStamp,
                        Level = loggingEvent.Level,
                        LoggerName = loggingEvent.LoggerName,
                        ThreadName = loggingEvent.ThreadName,
                        UserName = loggingEvent.UserName,
                        Identity = loggingEvent.Identity,
                        Domain = loggingEvent.Domain
                    };
                    base.Append(new LoggingEvent(data));
                }

                if (_cancellationToken.IsCancellationRequested)
                    break;

                Thread.Sleep(delay);
            }
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_latch)
                _queue.Enqueue(loggingEvent);
        }

        protected override void OnClose()
        {
            _done = true;
            base.OnClose();
        }

        public void Dispose()
        {
            if (!_worker.IsCompleted && _queue.Count > 0)
            {
                ProcessQueue(0);
                _cancellationToken.Cancel();
            }
        }
    }
}
