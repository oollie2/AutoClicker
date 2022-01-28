using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using AutoClicker.Bindings;
using AutoClicker.Classes;

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainBindings mainBindings;
        private GetInstances getInstances;
        private readonly Dictionary<Process, List<Clicker>> instanceClickers = new Dictionary<Process, List<Clicker>>();
        public MainWindow()
        {
            InitializeComponent();
            mainBindings = new();
            DataContext = mainBindings;
        }

        private void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (mainBindings == null)
                return;
            // First check an option is checked, otherwise there is nothing to do
            if(mainBindings.LeftTopCheckBox || mainBindings.RightTopCheckBox)
            {
                // Check the user has not deleted the data from the number selector
                if(mainBindings.LeftUpDownText != null && mainBindings.RightUpDownText != null)
                {
                    // Gather processes available and select one
                    getInstances = new();
                    if (getInstances.Check())
                    {
                        mainBindings.StartDateTime = "Started At: " + DateTime.Now.ToString("MMMM dd HH:mm tt");
                        mainBindings.StartDateTimeVisible = Visibility.Visible;
                        RunApplication();
                    }
                }
            }
        }

        private void RightButton_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }
        private void RunApplication()
        {
            foreach (Process process in getInstances.matchingProcesses)
            {

                IntPtr processHandle = process.MainWindowHandle;
                FocusToggle(processHandle);

                mainBindings.LeftButtonEnabled = false;
                mainBindings.LeftButtonContent = "Starting In: ";
                Thread.Sleep(500);

                mainBindings.LeftButtonContent += 5;
                Thread.Sleep(500);
                for (var i = 4; i > 0; i--)
                {
                    mainBindings.LeftButtonContent.Remove(mainBindings.LeftButtonContent.Length - 1);
                    mainBindings.LeftButtonContent += i;
                    Thread.Sleep(500);
                }

                mainBindings.LeftButtonContent = "Running...";
                Thread.Sleep(500);

                mainBindings.ApplicationEnabled = false;

                //Right click needs to be ahead of left click for concrete mining
                if (mainBindings.RightTopCheckBox)
                {
                    Classes.Button rightMouse = new(Win32Api.WmRbuttonDown, Win32Api.WmRbuttonDown + 1);
                    Clicker clicker = new(rightMouse, processHandle);
                    AddToInstanceClickers(process, clicker);
                    TimeSpan ts = TimeSpan.FromMilliseconds(Convert.ToInt32(mainBindings.RightUpDownText));
                    clicker.Start(ts);
                }

                /*
                 * This sleep is needed, because if you want to mine concrete, then Minecraft starts to hold left click first
                 * and it won't place the block in your second hand for some reason...
                 */
                Thread.Sleep(100);
                if (mainBindings.LeftTopCheckBox)
                {
                    Classes.Button leftMouse = new(Win32Api.WmLbuttonDown, Win32Api.WmLbuttonDown + 1);
                    Clicker clicker = new(leftMouse, processHandle);
                    AddToInstanceClickers(process, clicker);
                    TimeSpan ts = TimeSpan.FromMilliseconds(Convert.ToInt32(mainBindings.LeftUpDownText));
                    clicker.Start(ts);
                }
                mainBindings.RightButtonEnabled = true;
            }
        }
        private void Stop()
        {
            mainBindings.RightButtonEnabled = false;
            foreach (var clickers in instanceClickers.Values)
            {
                foreach (var clicker in clickers)
                {
                    clicker?.Dispose();
                }
            }

            instanceClickers.Clear();
            mainBindings.ApplicationEnabled = true;
            mainBindings.LeftButtonContent = "START";
            mainBindings.LeftButtonEnabled = true;
        }
        private void AddToInstanceClickers(Process mcProcess, Clicker clicker)
        {
            if (instanceClickers.ContainsKey(mcProcess))
                instanceClickers[mcProcess].Add(clicker);
            else
                instanceClickers.Add(mcProcess, new List<Clicker> { clicker });
        }
        private static void FocusToggle(IntPtr hwnd)
        {
            Thread.Sleep(200);
            Win32Api.SetForegroundWindow(hwnd);
        }
    }
}
