// -----------------------------------------------------------------------
// <copyright file="Log4NetWrapper.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using Core.Interface;
using log4net;

namespace Logger
{
    public sealed class Log4NetWrapper : BaseLogger, ILogger
    {
        #region Properties and ILog Properties

        private static log4net.ILog _log;

        public bool IsDebugEnabled { get { return (LogLevel <= LogLevelEnum.Debug); } }
        public bool IsInfoEnabled { get { return (LogLevel <= LogLevelEnum.Info); } }
        public bool IsWarnEnabled { get { return (LogLevel <= LogLevelEnum.Warn); } }
        public bool IsErrorEnabled { get { return (LogLevel <= LogLevelEnum.Error); } }
        public bool IsFatalEnabled { get { return (LogLevel <= LogLevelEnum.Fatal); } }

        #endregion

        #region Constructor Methods

        public Log4NetWrapper(LogLevelEnum loglevel)
        {
            this.LogLevel = loglevel;
            _log = LogManager.GetLogger("Log4NetLogger");
            log4net.Config.XmlConfigurator.Configure();
        }

        #endregion

        #region ILogger Implementations

        public void Debug(object message)
        {
            if (!IsDebugEnabled)
            {
                return;
            }
            _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            object[] args = {exception};

            DebugFormat((string)message, args);
        }

        public void DebugFormat(string message, params object[] args)
        {
            if (!IsDebugEnabled)
            {
                return;
            }
            _log.DebugFormat(message, args);
        }

        public void Info(object message)
        {
            if (!IsInfoEnabled)
            {
                return;
            }
            _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            object[] args = { exception };

            InfoFormat((string)message, args);
        }

        public void InfoFormat(string message, params object[] args)
        {
            if (!IsInfoEnabled)
            {
                return;
            }
            _log.InfoFormat(message, args);
        }

        public void Warn(object message)
        {
            if (!IsWarnEnabled)
            {
                return;
            }
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            object[] args = { exception };

            WarnFormat((string)message, args);
        }

        public void WarnFormat(string message, params object[] args)
        {
            if (!IsWarnEnabled)
            {
                return;
            }
            _log.WarnFormat(message, args);
        }

        public void Error(object message)
        {
            if (!IsErrorEnabled)
            {
                return;
            }
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            object[] args = { exception };

            ErrorFormat((string)message, args);
        }

        public void ErrorFormat(string message, params object[] args)
        {
            if (!IsErrorEnabled)
            {
                return;
            }
            _log.ErrorFormat(message, args);
        }

        public void Fatal(object message)
        {
            if (!IsFatalEnabled)
            {
                return;
            }
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            object[] args = { exception };

            FatalFormat((string)message, args);
        }

        public void FatalFormat(string message, params object[] args)
        {
            if (!IsFatalEnabled)
            {
                return;
            }
            _log.FatalFormat(message, args);
        }

        #endregion
        
    }
}
