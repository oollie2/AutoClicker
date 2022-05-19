using AutoClicker.Bindings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AutoClicker.Windows;
/// <summary>
/// Interaction logic for NoInstanceWindow.xaml
/// </summary>
public partial class NoInstanceWindow : Window
{
    public List<Process> ProcessSelected { get; private set; }
    private NoInstanceBindings noInstanceBindings;
    public NoInstanceWindow(NoInstanceBindings _noInstanceBindings)
    {
        InitializeComponent();
        noInstanceBindings = _noInstanceBindings;
        DataContext = noInstanceBindings;
    }

    private void ButtonOK_Click(object sender, RoutedEventArgs e)
    {
        ProcessSelected = Process.GetProcesses().Where(b => b.MainWindowTitle == noInstanceBindings.ProcessesText).ToList();
        if(ProcessSelected.Count < 1)
        {
            ProcessSelected = null;
            DialogResult = false;
        }
        else
        {
            DialogResult = true;
        }
        Close();
    }

    private void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}