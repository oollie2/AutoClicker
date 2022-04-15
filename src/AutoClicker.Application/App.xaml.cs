using System;
using System.IO;
using System.Windows;

namespace AutoClicker
{
    /// <summary>
    /// Auto Clicker main application start up.
    /// </summary>
    public partial class App : Application
    {
        private Classes.Logging Logger;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger = new();
            Logger.ExceptionLocation = Environment.ExpandEnvironmentVariables(@"%APPDATA%\oollie34\AutoClicker\Logs\");
            Logger.ExceptionLogging = true;
            Logger.DeleteOld();
            new MainWindow().Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Logger.ExceptionLogging = false;
            Logger = null;
            Current.Shutdown();
            Environment.Exit(0);
        }
    }
}
