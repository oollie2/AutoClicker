using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoClicker.Classes
{
    public class Logging
    {
        private Logger _logger;
        private bool _exceptionLogging;
        private LoggingConfiguration config;
        private string _exceptionLocation;
        /// <summary>
        /// Enable or disable all exception logging.
        /// </summary>
        public bool ExceptionLogging
        {
            get { return _exceptionLogging; }
            set { _exceptionLogging = value; ConfigureLogging(); }
        }
        /// <summary>
        /// What directory to commit exception logs to.
        /// </summary>
        public string ExceptionLocation
        {
            get { return _exceptionLocation; }
            set { _exceptionLocation = value; ConfigureLogging(); }
        }
        public Logging()
        {
            ExceptionLogging = false;
        }
        /// <summary>
        /// Function to delete log files older than 5 days.
        /// </summary>
        public void DeleteOld()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(ExceptionLocation);
            ArrayList files = new ArrayList();
            files.AddRange(dirinfo.GetFiles().OrderBy(x => x.CreationTime).ToArray());
            foreach (FileInfo file in files)
            {
                if ((DateTime.UtcNow - file.CreationTime).Days > 5)
                {
                    file.Delete();
                }
            }
        }
        /// <summary>
        /// With the variables defined in the class, set up the logging.
        /// This function also resets the logger first, is triggered by a change of the 
        /// bool or a change of the path.
        /// </summary>
        public void ConfigureLogging()
        {
            ResetLogger();
            if (ExceptionLogging)
            {
                config = new();
                Directory.CreateDirectory(ExceptionLocation);
                FileTarget logfile = new("EXCEPTION-LOG") { FileName = ExceptionLocation + "ExceptionLog-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt" };
                config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
                LogManager.Configuration = config;
                _logger = LogManager.GetLogger("EXCEPTION-LOG");
                SetupExceptionHandling();
            }
            else
            {
                _logger = null;
            }
        }
        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            System.Windows.Application.Current.DispatcherUnhandledException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
        }
        private void LogUnhandledException(Exception exception, string source)
        {
            string message = $"Unhandled exception ({source})";
            try
            {
                message = string.Format("\r\nUnhandled exception in {0} v{1}:\r\n\r\n" +
                    " - Source: " + source + "\r\n" +
                    " - Message: " + exception.Message + "\r\n" +
                    " - Data: " + exception.Data + "\r\n" +
                    " - Target Site: " + exception.TargetSite + "\r\n" +
                    " - Stack Trace: " + exception.StackTrace.Split(new[] { '\r', '\n' }).FirstOrDefault() + "\r\n",
                    Assembly.GetExecutingAssembly().GetName().Name,
                    Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception in LogUnhandledException");
            }
            finally
            {
                _logger.Error(exception, message);
            }
        }
        private void ResetLogger()
        {
            AppDomain.CurrentDomain.UnhandledException -= (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");
            System.Windows.Application.Current.DispatcherUnhandledException -= (s, e) =>
            {
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
                e.Handled = true;
            };
            TaskScheduler.UnobservedTaskException -= (s, e) =>
            {
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
                e.SetObserved();
            };
            config = null;
            _logger = null;
        }
    }
}
