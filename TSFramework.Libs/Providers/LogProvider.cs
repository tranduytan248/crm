using System;
using TSFramework.Libs.Models.Log;

namespace TSFramework.Libs.Providers
{
    public class LogProvider
    {
        private static LogWriter _logger;

        public LogProvider()
        {
            _logger = LogWriter.Instance;
        }

        public void Error(Exception ex)
        {
            _logger.WriteToLog(ex);
        }

        public void Message(string msg)
        {
            _logger.WriteToLog(msg);
        }
    }
}