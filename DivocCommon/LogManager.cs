using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using System.Diagnostics;
using System.Reflection;

namespace DivocCommon
{
    public static class LogManager
    {
        static Logger _log = null;

        static LogManager()
        {
            string name = Properties.Resource.ProductName;
            // Needs to have logic for determining which levels are actually desired to output.
            // Reading from a local config file would be desireable from a customer support
            // standpoint. Tell customer what to config and let them run the software to
            // generate new logs and have them send those for analysis.
            _log = new LoggerConfiguration()
                   .MinimumLevel.Debug()
                   .WriteTo.Console()
                   .WriteTo.Debug()
                   //.WriteTo.File(string.Format("logs\\{0} [Verbose].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose, rollingInterval: RollingInterval.Day)    // Ha ha no don't do this
                   .WriteTo.File(string.Format("logs\\{0} [Debug].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, rollingInterval: RollingInterval.Day)
                   .WriteTo.File(string.Format("logs\\{0} [Information].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Day)
                   .WriteTo.File(string.Format("logs\\{0} [Warning].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
                   .WriteTo.File(string.Format("logs\\{0} [Error].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
                   .WriteTo.File(string.Format("logs\\{0} [Fatal].log", name), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Fatal, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
        }

        public static void LogMethod(string message = "")
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);
            MethodBase mb = sf.GetMethod();
            _log.Information("[{type}.{method}] {message}", mb.DeclaringType.FullName, mb.Name, message);
        }

        public static void LogException(Exception ex)
        {
            _log.Error("{@ex}", ex);
        }

        public static void LogInformation(string message)
        {
            _log.Information(message);
        }
    }
}
