// -----------------------------------------------------------------------
// <copyright file="ConsoleLogger.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Globalization;
using Core.Interface;

namespace Logger
{
    public sealed class ConsoleLogger : BaseLogger, ILogger
    {
        #region Properties and ILog Properties

        public bool IsDebugEnabled { get { return (LogLevel <= LogLevelEnum.Debug); } }
        public bool IsInfoEnabled { get { return (LogLevel <= LogLevelEnum.Info); } }
        public bool IsWarnEnabled { get { return (LogLevel <= LogLevelEnum.Warn); } }
        public bool IsErrorEnabled { get { return (LogLevel <= LogLevelEnum.Error); } }
        public bool IsFatalEnabled { get { return (LogLevel <= LogLevelEnum.Fatal); } }

        #endregion

        #region Constructor Methods

        public ConsoleLogger(LogLevelEnum loglevel)
        {
            LogLevel = loglevel;
        }

         #endregion

        #region public Methods

        public void Log(LogTypeEnum logLevel, string logMessage, params object[] args)
        {
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, logMessage, args).ToString();
            Log(logLevel, message);
        }

         #endregion

        #region Private Methods

        private void Log(LogTypeEnum logLevel, string logMessage)
        {
            string prompt = string.Empty;

            switch (logLevel)
            {
                case LogTypeEnum.DEBUG:
                    prompt = "Debug: {0}";
                    break;
                case LogTypeEnum.ERROR:
                    prompt = "Error: {0}";
                    break;
                case LogTypeEnum.FATAL:
                    prompt = "Fatal: {0}";
                    break;
                case LogTypeEnum.INFO:
                    prompt = "Info: {0}";
                    break;
                case LogTypeEnum.WARN:
                    prompt = "Warn: {0}";
                    break;
                default:
                    throw new NotSupportedException();
            }

            System.Diagnostics.Debug.Write(String.Format(prompt, logMessage));
        }

        #endregion

        #region ILogger Implementations

        public void Debug(object message)
        {
            if (!IsDebugEnabled)
            {
                return;
            }
            Log(LogTypeEnum.DEBUG, (string)message, null);
        }

        public void Debug(object message, Exception exception)
        {
            DebugFormat((string)message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (!IsDebugEnabled)
            {
                return;
            }
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args).ToString();
            Log(LogTypeEnum.DEBUG, message, null);
        }

        public void Info(object message)
        {
            if (!IsInfoEnabled)
            {
                return;
            }
            Log(LogTypeEnum.INFO, (string)message, null);
        }

        public void Info(object message, Exception exception)
        {
            InfoFormat((string)message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (!IsInfoEnabled)
            {
                return;
            }
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args).ToString();
            Log(LogTypeEnum.INFO, message, null);
        }

        public void Warn(object message)
        {
            if (!IsWarnEnabled)
            {
                return;
            }
            Log(LogTypeEnum.WARN, (string)message, null);
        }

        public void Warn(object message, Exception exception)
        {
            WarnFormat((string)message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (!IsWarnEnabled)
            {
                return;
            }
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args).ToString();
            Log(LogTypeEnum.WARN, message, null);
        }

        public void Error(object message)
        {
            if (!IsErrorEnabled)
            {
                return;
            }
            Log(LogTypeEnum.ERROR, (string)message, null);
        }

        public void Error(object message, Exception exception)
        {
            ErrorFormat((string)message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (!IsErrorEnabled)
            {
                return;
            }
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args).ToString();
            Log(LogTypeEnum.ERROR, message, null);
        }

        public void Fatal(object message)
        {
            if (!IsFatalEnabled)
            {
                return;
            }
            Log(LogTypeEnum.FATAL, (string)message, null);
        }

        public void Fatal(object message, Exception exception)
        {
            FatalFormat((string)message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (!IsFatalEnabled)
            {
                return;
            }
            string message = new SystemStringFormat(CultureInfo.InvariantCulture, format, args).ToString();
            Log(LogTypeEnum.FATAL, message, null);
        }

        #endregion
        
    }
}
