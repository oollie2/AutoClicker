using AutoClicker.Classes;
using System;
using System.Diagnostics;
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
                @"\Settings\settings_test.xml");

#else
        SettingsFile = Environment.ExpandEnvironmentVariables(@"%APPDATA%\" +
            ((AssemblyCompanyAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyCompanyAttribute), false)).Company + @"\" +
            Assembly.GetExecutingAssembly().GetName().Name +
            @"\Settings\settings.xml");
#endif
            Settings = new(SettingsFile);

            if (e.Args.Length > 0 && e.Args[0] == "-update")
                UpdateApplication(e.Args[1]);

            new Windows.MainWindow().Show();

            Logger = new();
            

            if (Settings.Main.CheckForUpdates)
            {
                Updater updater = new();
                await updater.CheckVersionsAsync();
                if (updater.State == UpdateState.UpdateAvailable)
                {
                    MessageBoxResult update = MessageBox.Show("A new update is available, would you like to download and install?",
                        "Update Available",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                    if (update == MessageBoxResult.Yes)
                    {
                        updater.PerformUpdate();
                        return;
                    }
                }
                File.Delete(updater.DownloadLocation);
            }
        }
        /// <summary>
        /// Simply run an update in a seperate process and close the application.
        /// </summary>
        private static void UpdateApplication(string downloadPath)
        {
            if (File.Exists(downloadPath))
            {
                using Process installerProcess = new();
                ProcessStartInfo processInfo = new()
                {
                    FileName = "cmd.exe",
                    Arguments = "/C msiexec.exe /l*v \"" + Environment.ExpandEnvironmentVariables(Settings.Main.ExceptionLogLocation) + "upgrade.log\" /qn /i \"" + downloadPath + "\" && " +
                    "\"" + Environment.ProcessPath + "\"",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden,
                };
                installerProcess.StartInfo = processInfo;
                installerProcess.Start();
                Environment.Exit(0);
            }
            else
            {
                MessageBox.Show("An error has occurred running the update. Please restart the application and try again.",
                    "Unable to Update",
                    MessageBoxButton.OK,
                    MessageBoxImage.Exclamation);
                Environment.Exit(0);
            }
        }
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Settings.Dispose();
            Environment.Exit(0);
        }
    }
}
