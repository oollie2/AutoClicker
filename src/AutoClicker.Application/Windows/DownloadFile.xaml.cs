using System.Windows;

namespace AutoClicker.Windows
{
    /// <summary>
    /// Interaction logic for DownloadFile.xaml
    /// </summary>
    public partial class DownloadFile : Window
    {
        public DownloadFile(Bindings.DownloadBindings downloadBindings)
        {
            InitializeComponent();
            DataContext = downloadBindings;
        }
    }
}
