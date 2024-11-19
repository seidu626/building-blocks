using System.Reflection;
using Serilog;

namespace BuildingBlocks.Pipeline
{
    public static class MethodTimeLogger
    {
        private static ILogger _logger;

        public static void ConfigureLogger(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
                  
        public static void Log(MethodBase methodBase, TimeSpan timeSpan, string message)
        {
            if (_logger == null)
            {       
                throw new InvalidOperationException(
                    "Logger is not configured. Call ConfigureLogger to set the logger instance.");
            }

            _logger.Debug(
                "{Class}.{Method} - {Message} in {Duration}",
                methodBase.DeclaringType?.FullName,
                methodBase.Name,
                message,
                timeSpan);
        }
    }   
}