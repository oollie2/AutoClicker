using System.Windows;

namespace AutoClicker
{
    /// <summary>
    /// Main application start up code
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new MainWindow().Show();
        }
    }
}
