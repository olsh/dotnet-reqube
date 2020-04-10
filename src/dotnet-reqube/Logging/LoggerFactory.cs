using Serilog;

namespace ReQube.Logging
{
    internal static class LoggerFactory
    {
        private static readonly ILogger Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();

        public static ILogger GetLogger()
        {
            return Logger;
        }
    }
}
