// -----------------------------------------------------------------------
// <copyright file="BaseLogger.cs" company="Magic FireFly">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace Logger
{
    public class BaseLogger
    {
        public LogLevelEnum LogLevel { get; set; }

        private string TimeStamp
        {
            get
            {
                string timeStamp = string.Format("{0:HH:mm:ss.fff}", DateTime.Now);
                return timeStamp;
            }
        }
    }
}
