using System;
using TSFramework.Libs.Models.Log;

namespace TSFramework.Libs.Providers
{
    public class JobLogProvider
    {
        private static JobLogWriter _jobLogger;

        public JobLogProvider()
        {
            _jobLogger = JobLogWriter.GetInstance;
        }

        public void Message(string jobName, string msg)
        {
            _jobLogger.WriteToLog(jobName, msg);
        }

        public void Message(string jobName, Exception ex)
        {
            _jobLogger.WriteToLog(jobName, ex);
        }

        public void Error(string jobName, Exception ex)
        {
            _jobLogger.WriteToLog(jobName, ex);
        }
    }
}