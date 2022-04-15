using AutoClicker.Bindings;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace AutoClicker
{
    /// <summary>
    /// Interaction logic for NoInstanceWindow.xaml
    /// </summary>
    public partial class MultipleInstanceWindow : Window
    {
        public List<Process> ProcessSelected { get; private set; }
        private MultipleInstanceBindings multipleInstanceBindings;
        public MultipleInstanceWindow(MultipleInstanceBindings _multipleInstanceBindings)
        {
            InitializeComponent();
            multipleInstanceBindings = _multipleInstanceBindings;
            DataContext = multipleInstanceBindings;
            ProcessSelected = new();
        }
        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            foreach (string item in e.RemovedItems)
            {
                List<Process> toRemove = Process.GetProcesses().Where(b => b.MainWindowTitle == item).ToList();
                foreach(Process process in toRemove)
                {
                    ProcessSelected.Remove(process);
                }
            }
            foreach (string item in e.AddedItems)
            {
                List<Process> toAdd = Process.GetProcesses().Where(b => b.MainWindowTitle == item).ToList();
                foreach (Process process in toAdd)
                {
                    ProcessSelected.Add(process);
                }
            }
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
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
}
