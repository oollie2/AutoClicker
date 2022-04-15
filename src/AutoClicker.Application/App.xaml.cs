using AutoClicker.Classes;
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
        private Logging Logger;
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger = new();
            Logger.ExceptionLocation = Environment.ExpandEnvironmentVariables(@"%APPDATA%\oollie34\AutoClicker\Logs\");
            Logger.ExceptionLogging = true;
            Logger.DeleteOld();

            Updater updater = new();
            await updater.CheckVersionsAsync();
            if(updater.State == UpdateState.UpdateAvailable)
            {
                MessageBoxResult update = MessageBox.Show("A new update is available, would you like to download and install?",
                    "Update Check",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if(update == MessageBoxResult.Yes)
                {
                    updater.PerformUpdate();
                    return;
                }
            }
            File.Delete(updater.DownloadLocation);
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
