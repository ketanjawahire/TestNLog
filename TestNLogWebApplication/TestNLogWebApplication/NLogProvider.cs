using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TestNLogWebApplication
{
    public class NLogProvider : ILoggerProvider
    {
        private readonly string _nLogConfigFile;
        private readonly ConcurrentDictionary<string, NLogLogger> _loggers = new ConcurrentDictionary<string, NLogLogger>();

        public NLogProvider(string nLogConfigFile)
        {
            _nLogConfigFile = nLogConfigFile;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
        }

        public void Dispose()
        {
            _loggers.Clear();
        }
        private NLogLogger CreateLoggerImplementation(string name)
        {
            return new NLogLogger(name, _nLogConfigFile);
        }
    }
}