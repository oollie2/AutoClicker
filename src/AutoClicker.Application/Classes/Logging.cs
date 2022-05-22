using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoClicker.Classes;
public class Logging
{
    public Logger Log;
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
        ExceptionLogging = Settings.Main.ExceptionLogging;
        ExceptionLocation = Environment.ExpandEnvironmentVariables(Settings.Main.ExceptionLogLocation);
        Directory.CreateDirectory(Path.GetDirectoryName(ExceptionLocation));
        ConfigureLogging();
    }
    /// <summary>
    /// Function to delete log files older than 5 days.
    /// </summary>
    public void DeleteOld()
    {
        if(Settings.Main.ExceptionLogDays > 0)
        {
            DirectoryInfo dirinfo = new DirectoryInfo(ExceptionLocation);
            ArrayList files = new ArrayList();
            files.AddRange(dirinfo.GetFiles().OrderBy(x => x.CreationTime).ToArray());
            foreach (FileInfo file in files)
            {
                if ((DateTime.UtcNow - file.CreationTime).Days > Settings.Main.ExceptionLogDays)
                {
                    file.Delete();
                }
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
            FileTarget logfile = new("EXCEPTION-LOG")
            {
                FileName = Path.Join(ExceptionLocation,
                "ExceptionLog-" + DateTime.Now.ToString("dd-MMM-yyyy") + ".txt")
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;
            Log = LogManager.GetLogger("EXCEPTION-LOG");
            SetupExceptionHandling();
        }
        else
        {
            Log = null;
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
        string version = "";
#if DEBUG
        version = "AutoClicker " + Assembly.GetEntryAssembly().GetName().Version.Major + "." +
Assembly.GetEntryAssembly().GetName().Version.Minor + "." + Assembly.GetEntryAssembly().GetName().Version.Build + " Debug";
#else
        version = "AutoClicker " + Assembly.GetEntryAssembly().GetName().Version.Major + "." +
            Assembly.GetEntryAssembly().GetName().Version.Minor + "." + Assembly.GetEntryAssembly().GetName().Version.Build;
#endif
        try
        {
            if (exception.Source != null && exception.StackTrace != null && exception.TargetSite != null)
            {
                message = string.Format("\r\nUnhandled exception in {0} v{1}:\r\n\r\n" +
                    " - Source: " + source + "\r\n" +
                    " - Message: " + exception.Message + "\r\n" +
                    " - Data: " + exception.Data + "\r\n" +
                    " - Target Site: " + exception.TargetSite + "\r\n" +
                    " - Stack Trace: " + exception.StackTrace.Split(new[] { '\r', '\n' }).FirstOrDefault() + "\r\n",
                    " - Parameter Name: " + exception.Source + "\r\n" +
                    Assembly.GetExecutingAssembly().GetName().Name,
                    version);
            }
            else
            {
                message = string.Format("\r\nUnhandled exception in {0} v{1}:\r\n\r\n" +
                    " - Message: " + exception.Message + "\r\n" +
                    " - Data: " + exception.Data + "\r\n" +
                    " - Parameter Name: " + exception.Source + "\r\n" +
                    Assembly.GetExecutingAssembly().GetName().Name,
                    version);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Exception in LogUnhandledException");
        }
        finally
        {
            Log.Error(exception, message);
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
        Log = null;
    }
}