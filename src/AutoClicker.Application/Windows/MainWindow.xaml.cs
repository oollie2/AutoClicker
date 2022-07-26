using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using AutoClicker.Bindings;
using AutoClicker.Classes;

namespace AutoClicker.Windows;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainBindings MainBindings { get; set; }
    private readonly Dictionary<Process, List<Clicker>> instanceClickers = new();
    private GetInstances Instances;
    private readonly Hotkeys hotkeys;
    private bool Started = false;
    public MainWindow()
    {
        InitializeComponent();
        MainBindings = new();
        DataContext = MainBindings;
        hotkeys = new();
        hotkeys.Play += Hotkey_Play;
        hotkeys.Pause += Hotkey_Pause;
    }
    private void Hotkey_Pause()
    {
        if (Started)
            Stop();
    }
    private void Hotkey_Play()
    {
        if (!Started)
            InitStart();
    }
    private void LeftButton_Click(object sender, RoutedEventArgs e)
    {
        InitStart();
    }
    private void InitStart()
    {
        if (MainBindings == null)
            return;
        // First check an option is checked, otherwise there is nothing to do
        if (CheckBools())
        {
            // Check the user has not deleted the data from the number selector
            if (MainBindings.LeftUpDownText != null && MainBindings.RightUpDownText != null)
            {
                // Gather processes available and select one
                Instances = new();
                if (Instances.Check())
                {
                    DelayedStart(Settings.Main.MillisecondStartDelay);
                }
            }
            else
            {
                MainBindings.IndicatorLabel = "Not started - no delay selected.";
            }
        }
        else
        {
            MainBindings.IndicatorLabel = "Not started - no selections made.";
        }
    }
    private void Start()
    {
        Started = true;
        MainBindings.IndicatorLabel = "Started At: " + DateTime.Now.ToString("MMMM dd HH:mm tt");
        MainBindings.IndicatorLabelVisible = Visibility.Visible;
        RunApplication();
    }
    private async void DelayedStart(int millisecondsDelay)
    {
        long tickStop = Environment.TickCount + millisecondsDelay;
        while (Environment.TickCount < tickStop)
        {
            await Task.Run(() =>
            {
                MainBindings.IndicatorLabel = "Starting in " + Math.Round((double)(tickStop - Environment.TickCount) / 1000, 0) + " Seconds";
            });
        }
        Start();
    }
    private bool CheckBools()
    {
        List<bool> bools = new()
        {
            MainBindings.LeftTopCheckBox,
            MainBindings.LeftBottomCheckBox,
            MainBindings.RightTopCheckBox,
            MainBindings.RightBottomCheckBox
        };
        if (bools.IndexOf(true) > -1) return true;
        else return false;

    }
    private void RightButton_Click(object sender, RoutedEventArgs e)
    {
        Stop();
    }
    private void RunApplication()
    {
        if(Instances != null)
        {
            foreach (Process process in Instances.matchingProcesses)
            {

                IntPtr processHandle = process.MainWindowHandle;
                FocusToggle(processHandle);

                MainBindings.LeftButtonEnabled = false;
                MainBindings.LeftButtonContent = "Starting In: ";
                MainBindings.LeftButtonContent += 5;
                for (var i = 4; i > 0; i--)
                {
                    MainBindings.LeftButtonContent.Remove(MainBindings.LeftButtonContent.Length - 1);
                    MainBindings.LeftButtonContent += i;
                }

                MainBindings.LeftButtonContent = "Running...";
                MainBindings.ApplicationEnabled = false;

                if (MainBindings.RightTopCheckBox || MainBindings.RightBottomCheckBox)
                {
                    Clicker clicker = new(Win32Api.WmRbuttonDown, Win32Api.WmRbuttonDown + 1, processHandle);
                    AddToInstanceClickers(process, clicker);
                    clicker.Start(Convert.ToDouble(MainBindings.RightUpDownText));
                }

                Thread.Sleep(100);
                if (MainBindings.LeftTopCheckBox || MainBindings.LeftBottomCheckBox)
                {
                    Clicker clicker = new(Win32Api.WmLbuttonDown, Win32Api.WmLbuttonDown + 1, processHandle);
                    AddToInstanceClickers(process, clicker);
                    clicker.Start(Convert.ToDouble(MainBindings.LeftUpDownText));
                }
                MainBindings.RightButtonEnabled = true;
            }
        }
    }
    private void Stop()
    {
        Started = false;
        MainBindings.RightButtonEnabled = false;
        foreach (var clickers in instanceClickers.Values)
        {
            foreach (var clicker in clickers)
            {
                clicker?.Dispose();
            }
        }

        instanceClickers.Clear();
        Instances = null;
        MainBindings.ApplicationEnabled = true;
        MainBindings.LeftButtonContent = "START";
        MainBindings.LeftButtonEnabled = true;
        MainBindings.IndicatorLabel = "Idle";
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
    private void Window_Closed(object sender, EventArgs e)
    {
        hotkeys.Dispose();
    }
}