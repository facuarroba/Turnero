using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TurneroClassLibrary
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _logger;

        public NLogLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(Exception x)
        {
            _logger.Error(this.BuildExceptionMessage(x));
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception x)
        {
            _logger.Fatal(this.BuildExceptionMessage(x));
        }

        public static void ConfigureLogger(String path="C:\\", String appName = "MyApp")
        {
            // Step 1. Create configuration object 
            LoggingConfiguration config = new LoggingConfiguration();

            FileTarget fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 

            var logPath = path;

            fileTarget.FileName = Path.Combine(logPath, appName + ".log");
            fileTarget.ArchiveFileName = Path.Combine(logPath, appName + ".{#####}.txt");
            fileTarget.ArchiveAboveSize = 10240; // 10kb
            fileTarget.ArchiveNumbering = ArchiveNumberingMode.Sequence;
            fileTarget.ConcurrentWrites = true;
            fileTarget.KeepFileOpen = false;

            fileTarget.Layout = "${longdate} | ${level} | ${message}";

            LoggingRule rule2 = new LoggingRule("*", LogLevel.Info, fileTarget);
            config.LoggingRules.Add(rule2);

            // Step 5. Activate the configuration
            LogManager.Configuration = config;
        }
    }

    public static class LoggerExt
    {
        public static string BuildExceptionMessage(this ILogger logger, Exception x)
        {
            Exception logException = x;
            if (x.InnerException != null)
                logException = x.InnerException;

            string strErrorMsg = Environment.NewLine + "Message :" + logException.Message;

            strErrorMsg += Environment.NewLine + "Source :" + logException.Source;

            strErrorMsg += Environment.NewLine + "Stack Trace :" + logException.StackTrace;

            strErrorMsg += Environment.NewLine + "TargetSite :" + logException.TargetSite;
            return strErrorMsg;
        }
    }
}
