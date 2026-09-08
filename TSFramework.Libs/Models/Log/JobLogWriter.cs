using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace TSFramework.Libs.Models.Log
{
    public class JobLogWriter
    {
        /// <summary>
        ///     Single instance of logwriter
        /// </summary>
        private static JobLogWriter _instance;

        /// <summary>
        ///     Queue used to store logs
        /// </summary>
        private static Queue<LogModel> _logQueue;

        /// <summary>
        ///     Path to save log files
        /// </summary>
        private const string LOG_PATH = "Contents/JobLogs"; //ConfigurationManager.AppSettings["LogPath"];

        /// <summary>
        ///     Lof file name
        /// </summary>
        private const string LOG_FILE = "JobLogsFile.log"; //ConfigurationManager.AppSettings["LogFile"];

        /// <summary>
        ///     Flush log when time reached
        /// </summary>
        private static readonly int flushAtAge = int.Parse(ConfigurationManager.AppSettings["FlushAtAge"]);

        /// <summary>
        ///     Flush log when quantity reached
        /// </summary>
        private static readonly int flushAtQty = int.Parse(ConfigurationManager.AppSettings["FlushAtQty"]);

        /// <summary>
        ///     Timestamp of last flush
        /// </summary>
        private static DateTime _flushedAt;

        /// <summary>
        ///     Private constructor -> prevent instantiation
        /// </summary>
        private JobLogWriter()
        {
        }

        /// <summary>
        ///     Returns static instance of writer
        /// </summary>
        public static JobLogWriter GetInstance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new JobLogWriter();
                _logQueue = new Queue<LogModel>();
                _flushedAt = DateTime.Now;

                return _instance;
            }
        }

        /// <summary>
        ///     Log message
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="message">Message to log</param>
        public void WriteToLog(string jobName, string message)
        {
            lock (_logQueue)
            {
                // Create log
                var log = new LogModel(message);
                _logQueue.Enqueue(log);

                // Check if should flush
                if (_logQueue.Count >= flushAtQty || CheckTimeToFlush()) FlushLogToFile(jobName);
            }
        }

        /// <summary>
        ///     Log exception
        /// </summary>
        /// <param name="jobName"></param>
        /// <param name="e">Exception to log</param>
        public void WriteToLog(string jobName, Exception e)
        {
            lock (_logQueue)
            {
                // Create log
                var msg = new LogModel(e.Source.Trim() + " " + e.Message.Trim());
                var stack = new LogModel("Stack: " + e.StackTrace.Trim());
                _logQueue.Enqueue(msg);
                _logQueue.Enqueue(stack);

                // Check if should flush
                if (_logQueue.Count >= flushAtQty || CheckTimeToFlush()) FlushLogToFile(jobName);
            }
        }

        /// <summary>
        ///     Force flush of log queue
        /// </summary>
        public static void ForceFlush(string jobName)
        {
            FlushLogToFile(jobName);
        }

        /// <summary>
        ///     Check if time to flush to file
        /// </summary>
        /// <returns></returns>
        private static bool CheckTimeToFlush()
        {
            var time = DateTime.Now - _flushedAt;
            if (!(time.TotalSeconds >= flushAtAge)) return false;
            _flushedAt = DateTime.Now;
            return true;
        }

        /// <summary>
        ///     Flush log queue to file
        /// </summary>
        private static void FlushLogToFile(string jobName)
        {
            lock (_logQueue)
            {
                while (_logQueue.Count > 0)
                {
                    // Get entry to log
                    var dir = HttpRuntime.AppDomainAppPath;
                    var entry = _logQueue.Dequeue();
                    if (!Directory.Exists(Path.Combine(dir, LOG_PATH)))
                        Directory.CreateDirectory(Path.Combine(dir, LOG_PATH));
                    var path = Path.Combine(Path.Combine(dir, LOG_PATH), entry.GetDate() + "_" + jobName + "_" + LOG_FILE);

                    // Crete filestream
                    File.AppendAllText(path,
                        $"{entry.GetTime()} {entry.GetMessage()}" + Environment.NewLine);
                }
            }
        }
    }
}
