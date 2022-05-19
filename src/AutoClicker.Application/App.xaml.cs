using AutoClicker.Classes;
using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace AutoClicker
{
    /// <summary>
    /// Auto Clicker main application start up.
    /// </summary>
    public partial class App : Application
    {
        public static Settings Settings { get; set; }
        public static string SettingsFile { get; set; }
        private Logging Logger { get; set; }
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            //Set up custom settings as everything else reads from that
#if DEBUG
            SettingsFile = Environment.ExpandEnvironmentVariables(@"%APPDATA%\" +
                ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company + @"\" +
                Assembly.GetExecutingAssembly().GetName().Name +
                @"\Settings\settings-debug.xml");

#else
        SettingsFile = Environment.ExpandEnvironmentVariables(@"%APPDATA%\" +
            ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company + @"\" +
            Assembly.GetExecutingAssembly().GetName().Name +
            @"\Settings\settings.xml");
#endif
            Settings = new(SettingsFile);

            new MainWindow().Show();

            Logger = new();

            Updater updater = new();
            await updater.CheckVersionsAsync();
            if(updater.State == UpdateState.UpdateAvailable)
            {
                MessageBoxResult update = MessageBox.Show("A new update is available, would you like to download and install?",
                    "Update Available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if(update == MessageBoxResult.Yes)
                {
                    updater.PerformUpdate();
                    return;
                }
            }
            File.Delete(updater.DownloadLocation);
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
