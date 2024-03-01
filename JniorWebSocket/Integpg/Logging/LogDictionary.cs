using System.Collections.Generic;

namespace Integpg.Logging
{
    public static class LogDictionary
    {
        private static readonly NLog.Logger EmptyLog = NLog.LogManager.GetLogger("Application");
        private static readonly Dictionary<object, NLog.Logger> Loggers = new Dictionary<object, NLog.Logger>();



        public static NLog.Logger GetLog(object loggerParent)
        {
            lock (Loggers)
            {
                if (null == loggerParent) return EmptyLog;
                if (!Loggers.ContainsKey(loggerParent))
                {
                    var logger = AddLog(loggerParent, loggerParent.ToString());
                    logger.Info("**************************************************");
                }
                return Loggers[loggerParent];
            }
        }



        private static NLog.Logger AddLog(object loggerParent, string loggerName)
        {
            lock (Loggers)
            {
                var logger = CreateLog(loggerName);
                Loggers.Add(loggerParent, logger);
                return logger;
            }
        }



        private static NLog.Logger CreateLog(string loggerName)
        {
            var logger = NLog.LogManager.GetLogger(loggerName);
            return logger;
        }
    }
}