using System;
using NLog;
using NLog.Layouts;
using NLog.Targets;

namespace CyberSource.Authentication.Util
{
    public class LogUtility
    {
        private static LogUtility _singletonLogUtility;

        private LogUtility(
            string enableLog,
            string logDirectory,
            string logFileName,
            string logFileMaxSize)
        {
            if (string.Equals(enableLog, "FALSE", StringComparison.OrdinalIgnoreCase))
                LogManager.DisableLogging();
            try
            {
                FileTarget targetByName = LogManager.get_Configuration().FindTargetByName("file") as FileTarget;
                if (targetByName == null)
                    throw new Exception(string.Format("{0} No Target with the name 'file' found in NLog.config",
                        (object) Constants.ErrorPrefix));
                if (!string.IsNullOrEmpty(logDirectory))
                    targetByName.set_FileName(Layout.op_Implicit(logDirectory + "\\" + logFileName));
                if (string.IsNullOrEmpty(logFileMaxSize))
                    return;
                targetByName.set_ArchiveAboveSize(long.Parse(logFileMaxSize));
            }
            catch (NullReferenceException ex)
            {
                LogManager.DisableLogging();
            }
        }

        public static void InitLogConfig(
            string enableLog,
            string logDirectory,
            string logFileName,
            string logFileMaxSize)
        {
            if (!string.Equals(enableLog, "true", StringComparison.OrdinalIgnoreCase))
                enableLog = "FALSE";
            if (LogUtility._singletonLogUtility != null)
                return;
            LogUtility._singletonLogUtility = new LogUtility(enableLog, logDirectory, logFileName, logFileMaxSize);
        }
    }
}
