using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace TSFramework.Libs.Models.Log
{
    public class LogWriter
    {
        /// <summary>
        ///     Single instance of logwriter
        /// </summary>
        private static LogWriter _instance;

        /// <summary>
        ///     Queue used to store logs
        /// </summary>
        private static Queue<LogModel> _logQueue;

        /// <summary>
        ///     Path to save log files
        /// </summary>
        private static readonly string logPath = ConfigurationManager.AppSettings["LogPath"] ?? "Logs";

        /// <summary>
        ///     Lof file name
        /// </summary>
        private static readonly string logFile = ConfigurationManager.AppSettings["LogFile"] ?? "LogsFile.log";

        /// <summary>
        ///     Flush log when time reached
        /// </summary>
        private static readonly int flushAtAge = int.Parse(ConfigurationManager.AppSettings["FlushAtAge"] ?? "500");

        /// <summary>
        ///     Flush log when quantity reached
        /// </summary>
        private static readonly int flushAtQty = int.Parse(ConfigurationManager.AppSettings["FlushAtQty"] ?? "1");

        /// <summary>
        ///     Timestamp of last flush
        /// </summary>
        private static DateTime _flushedAt;

        /// <summary>
        ///     Private constructor -> prevent instantiation
        /// </summary>
        private LogWriter()
        {
            _logQueue = new Queue<LogModel>();
        }

        /// <summary>
        ///     Returns static instance of writer
        /// </summary>
        public static LogWriter Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new LogWriter();
                lock (_logQueue)
                {
                    _logQueue = new Queue<LogModel>();
                }

                _flushedAt = DateTime.Now;

                return _instance;
            }
        }

        /// <summary>
        ///     Log message
        /// </summary>
        /// <param name="message">Message to log</param>
        public void WriteToLog(string message)
        {
            // Create log
            var log = new LogModel(message);
            _logQueue.Enqueue(log);

            // Check if should flush
            if (_logQueue.Count >= flushAtQty || CheckTimeToFlush()) FlushLogToFile();
        }

        /// <summary>
        ///     Log exception
        /// </summary>
        /// <param name="e">Exception to log</param>
        public void WriteToLog(Exception e)
        {
            // Create log
            var msg = new LogModel(e.Source?.Trim() + " " + e.Message?.Trim());
            var stack = new LogModel("Stack: " + e.StackTrace?.Trim());
            _logQueue.Enqueue(msg);
            _logQueue.Enqueue(stack);

            // Check if should flush
            if (_logQueue.Count >= flushAtQty || CheckTimeToFlush()) FlushLogToFile();
        }

        /// <summary>
        ///     Force flush of log queue
        /// </summary>
        public static void ForceFlush()
        {
            FlushLogToFile();
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
        private static void FlushLogToFile()
        {
            while (_logQueue.Count > 0)
            {
                // Get entry to log
                var dir = HttpRuntime
                    .AppDomainAppPath;
                var entry = _logQueue.Dequeue();
                if (!Directory.Exists(Path.Combine(dir, logPath)))
                    Directory.CreateDirectory(Path.Combine(dir, logPath));
                var path = Path.Combine(Path.Combine(dir, logPath), entry.GetDate() + "_" + logFile);

                // Crete filestream
                File.AppendAllText(path,
                    $"{entry.GetTime()} {entry.GetMessage()}" + Environment.NewLine);
            }
        }
    }
}