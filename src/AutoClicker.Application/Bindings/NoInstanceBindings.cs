using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace AutoClicker.Bindings;
public class NoInstanceBindings : INotifyPropertyChanged
{
    public NoInstanceBindings() 
    {
        mainData = new()
        {
            TitleText = "No Client Found",
            ProcessesText = "Select a Process",
            BodyText = "No game window was found. This may mean you're using a modded client. If you're sure the window is running, please select it from the processes listed below.",
            Processes = Process.GetProcesses().Where(b => b.ProcessName.StartsWith("java")).OrderBy(b => b.MainWindowTitle).Select(b => b.MainWindowTitle).ToList()
    };
    }
    private NoInstanceData mainData;
    public string TitleText
    {
        get
        {
            if (mainData != null)
                return mainData.TitleText;
            else
                return "";
        }
        set
        {
            if (TitleText != value)
                mainData.TitleText = value;
            OnPropertyChanged(nameof(TitleText));
        }
    }
    public string BodyText
    {
        get
        {
            if (mainData != null)
                return mainData.BodyText;
            else
                return "";
        }
        set
        {
            if (BodyText != value)
                mainData.BodyText = value;
            OnPropertyChanged(nameof(BodyText));
        }
    }
    public string ProcessesText
    {
        get
        {
            if (mainData != null)
                return mainData.ProcessesText;
            else
                return "";
        }
        set
        {
            if (ProcessesText != value)
                mainData.ProcessesText = value;
            OnPropertyChanged(nameof(ProcessesText));
        }
    }
    public List<string> Processes
    {
        get
        {
            if (mainData != null)
                return mainData.Processes;
            else
                return null;
        }
        set
        {
            if (Processes != value)
                mainData.Processes = value;
            OnPropertyChanged(nameof(Processes));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
sealed class NoInstanceData
{
    public string TitleText { get; set; }
    public string BodyText { get; set; }
    public string ProcessesText { get; set; }
    public List<string> Processes { get; set; }
}