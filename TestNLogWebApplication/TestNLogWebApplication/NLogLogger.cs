using System;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace TestNLogWebApplication
{
    public class NLogLogger : Microsoft.Extensions.Logging.ILogger
    {
        private readonly string _configFile;
        private readonly string _name;
        private readonly Logger _logger;

        public NLogLogger(string name, string configFile)
        {
            _name = name;
            _configFile = configFile;
            _logger = NLogBuilder.ConfigureNLog(_configFile).GetLogger(name);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return _logger.IsFatalEnabled;
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                case Microsoft.Extensions.Logging.LogLevel.Trace:
                    return _logger.IsDebugEnabled;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return _logger.IsErrorEnabled;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return _logger.IsInfoEnabled;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return _logger.IsWarnEnabled;
                default:
                    throw new ArgumentOutOfRangeException(nameof(logLevel));
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);

            if (string.IsNullOrEmpty(message)) return;

            var logEventInfo = new LogEventInfo() {LoggerName = _name, Message = message};
            logEventInfo.Properties.Add("EventId", eventId.Id);

            switch (logLevel)
            {
                case LogLevel.Critical:
                    logEventInfo.Level = NLog.LogLevel.Fatal;
                    logEventInfo.Exception = exception;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.Debug:
                    logEventInfo.Level = NLog.LogLevel.Debug;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.Trace:
                    logEventInfo.Level = NLog.LogLevel.Trace;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.Error:
                    logEventInfo.Level = NLog.LogLevel.Error;
                    logEventInfo.Exception = exception;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.Information:
                    logEventInfo.Level = NLog.LogLevel.Info;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.Warning:
                    logEventInfo.Level = NLog.LogLevel.Warn;
                    _logger.Log(logEventInfo);
                    break;
                case LogLevel.None:
                    break;
                default:
                    _logger.Warn($"Encountered unknown log level {logLevel}, writing out as Info.");
                    logEventInfo.Level = NLog.LogLevel.Info;
                    logEventInfo.Exception = exception;
                    _logger.Log(logEventInfo);
                    break;
            }
        }
    }
}